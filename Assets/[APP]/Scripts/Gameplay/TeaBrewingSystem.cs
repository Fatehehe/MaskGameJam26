using System;
using System.Collections.Generic;
using UnityEngine;

public enum BrewingStatus
{
    Incomplete, // Not enough ingredients, but the direction is correct
    Brewable,   // Recipe is correct!
    Ruined      // Recipe is wrong/incorrect
}

public class TeaBrewingSystem
{
    // Constants
    private const float CRACKED_THRESHOLD = 50f;
    private const float BROKEN_THRESHOLD = 100f;
    private const int MAX_ESSENCES_IN_POT = 3;

    private readonly ActiveCustomerProvider customerProvider;
    private readonly TeaDatabase teaDatabase;

    private List<TeaEssenceData> currentPot = new List<TeaEssenceData>();

    public event Action OnTeaServed;
    public event Action<bool> OnInteractableStateChanged;
    public event Action<List<TeaEssenceData>, BrewingStatus> OnPotUpdated;

    public TeaBrewingSystem(ActiveCustomerProvider customerProvider, TeaDatabase teaDatabase)
    {
        this.customerProvider = customerProvider;
        this.teaDatabase = teaDatabase;
    }

    public void AddEssenceToPot(TeaEssenceData essence)
    {
        if (currentPot.Count >= MAX_ESSENCES_IN_POT) return; 

        currentPot.Add(essence);
        
        BrewingStatus status = CheckRecipeStatus(currentPot);

        Debug.Log($"Added {essence.name}. Status: {status}");
        OnPotUpdated?.Invoke(currentPot, status);
    }

    public void ClearPot()
    {
        currentPot.Clear();
        OnPotUpdated?.Invoke(currentPot, BrewingStatus.Incomplete);
    }

    public void BrewAndServe()
    {
        if (!customerProvider.HasCustomer) return;
        if (currentPot.Count == 0) return;

        TeaData resultTea = FindMatchingRecipe(currentPot);
        
        if (resultTea != null)
        {
            ServeFinalTea(resultTea);
        }
        else
        {
            Debug.LogError("Error: Status is Brewable but recipe is null!");
            ClearPot();
        }
    }

    // Helper function to check if List A contains EXACTLY the same items as List B (Order doesn't matter)
    private bool AreEssenceListsEqual(List<TeaEssenceData> recipeList, List<TeaEssenceData> potList)
    {
        if (recipeList.Count != potList.Count) return false;

        // Copy the recipe list to a temp list so we can remove items
        List<TeaEssenceData> tempRecipe = new List<TeaEssenceData>(recipeList);

        for (int i = 0; i < potList.Count; i++)
        {
            TeaEssenceData itemInPot = potList[i];
            
            // Try to remove this item from tempRecipe
            // If false, it means the item in the pot is not in the recipe -> Different
            if (!tempRecipe.Remove(itemInPot)) 
            {
                return false;
            }
        }

        // If all items were successfully removed, the contents are the same
        return true;
    }

    // Helper function to check if the pot is a SUBSET of the recipe
    private bool IsPotSubsetOfRecipe(List<TeaEssenceData> recipeList, List<TeaEssenceData> potList)
    {
        // Copy recipe to temp
        List<TeaEssenceData> tempRecipe = new List<TeaEssenceData>(recipeList);

        for (int i = 0; i < potList.Count; i++)
        {
            TeaEssenceData itemInPot = potList[i];
            
            // If the item in the pot is not in the recipe -> Already off track (Ruined)
            if (!tempRecipe.Remove(itemInPot))
            {
                return false;
            }
        }

        // All pot items are in the recipe, but the recipe still has remaining items (means Incomplete)
        return true;
    }

    private BrewingStatus CheckRecipeStatus(List<TeaEssenceData> input)
    {
        var allRecipes = teaDatabase.GetAllItems();
        bool possibleMatchFound = false;

        for (int i = 0; i < allRecipes.Length; i++)
        {
            var recipe = allRecipes[i];
            var required = recipe.RequiredEssences;

            // 1. Check Exact Match (Ready to Brew)
            if (required.Count == input.Count)
            {
                if (AreEssenceListsEqual(required, input))
                {
                    return BrewingStatus.Brewable;
                }
            }

            // 2. Check Potential Match (Still on-track)
            // Only check recipes that have more ingredients than the current pot
            if (required.Count > input.Count)
            {
                if (IsPotSubsetOfRecipe(required, input))
                {
                    possibleMatchFound = true;
                }
            }
        }

        return possibleMatchFound ? BrewingStatus.Incomplete : BrewingStatus.Ruined;
    }

    private TeaData FindMatchingRecipe(List<TeaEssenceData> inputEssences)
    {
        var allRecipes = teaDatabase.GetAllItems();
        
        for (int i = 0; i < allRecipes.Length; i++)
        {
            var recipe = allRecipes[i];
            
            // Use the manual function above
            if (AreEssenceListsEqual(recipe.RequiredEssences, inputEssences))
            {
                return recipe;
            }
        }
        return null;
    }

    private void ServeFinalTea(TeaData tea)
    {
        var customer = customerProvider.CurrentCustomer;
        customer.TeaHistory.Add(tea); 

        float impact = tea.CrackImpact;
        if (tea.EmotionType == customer.ActiveMaskData.EmotionType)
        {
            impact *= 1.5f; 
        }

        customer.CurrentCrackPoints += impact;
        UpdateMaskState(customer);

        ClearPot();
        OnTeaServed?.Invoke();
    }
    
    public void SetInteractable(bool state) 
    {
        if (!state) ClearPot();
        OnInteractableStateChanged?.Invoke(state);
    }
    
    private void UpdateMaskState(ActiveCustomer customer)
    {
        float percent = customer.CrackPercentage;

        if (percent >= BROKEN_THRESHOLD)
            customer.CurrentState = MaskState.Broken;
        else if (percent >= CRACKED_THRESHOLD)
            customer.CurrentState = MaskState.Cracked;
        else
            customer.CurrentState = MaskState.Intact;
    }
}