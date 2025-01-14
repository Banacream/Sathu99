using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "new Recepi", menuName = "Cooking/Create New Recipe", order = 0)]
public class CookRecipe : ScriptableObject
{
    //[Header("Recipe")]
    //public DataItem[] recipeItems = new DataItem[0];
    [Header("Output Item")]
    public DataItem outputItem;
    public int outputStack = 1;

    //[Header("Recipe")]
    [System.Serializable]
    public struct RecipeIngredient
    {
        public DataItem ingredient; // �ѵ�شԺ�����
        public int quantity; // �ӹǹ����ͧ���

        public RecipeIngredient(DataItem ingredient, int quantity)
        {
            this.ingredient = ingredient;
            this.quantity = quantity;
        }
    }

    [SerializeField] public RecipeIngredient[] recipeItems = new RecipeIngredient[0]; // ��¡���ѵ�شԺ����ͧ��

    // �ӹǹ�ѵ�شԺ��������ٵ�
    public int NumberOfIngredients => recipeItems.Length;

    // �֧�������ѵ�شԺ�ҡ���˹觷���˹�
    public DataItem GetIngredientAt(int index)
    {
        if (index >= recipeItems.Length)
        {
            Debug.LogError("There is no ingredient at that index. Recipe: " + name);
            return null;
        }

        return recipeItems[index].ingredient;
    }

    // �֧�ӹǹ����ͧ��âͧ�ѵ�شԺ�ҡ���˹觷���˹�
    public int GetQuantityAt(int index)
    {
        if (index >= recipeItems.Length)
        {
            Debug.LogError("There is no ingredient at that index. Recipe: " + name);
            return -1;
        }

        return recipeItems[index].quantity;
    }

    // �����ѵ�شԺ����ŧ��ٵ�
    public void AddIngredient(DataItem ingredient, int quantity)
    {
        RecipeIngredient[] copy = new RecipeIngredient[recipeItems.Length + 1];
        recipeItems.CopyTo(copy, 0);

        copy[recipeItems.Length] = new RecipeIngredient(ingredient, quantity);

        recipeItems = copy;
    }



}
