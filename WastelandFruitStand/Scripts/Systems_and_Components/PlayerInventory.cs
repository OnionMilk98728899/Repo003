using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerInventory : Node2D
{
    [Export] private int  maxFruitQuan, maxCurrencyQuan, maxCupsQuantity;
    private int fruitQuantity, currencyQuantity;
    private int[] cupStorage;

    public override void _Ready()
    {
        cupStorage = new int[3];
        cupStorage[0] = 10;
    }
    public int GetFruitQuantity()
    {
        return fruitQuantity;
    }
    public void SetFruitQuantity(int fruit)
    {
        fruitQuantity = fruit;
    }
    public void AddFruit(int fruit)
    {
        fruitQuantity += fruit;
    }

    public int GetCurrencyQuantity()
    {
        return currencyQuantity;
    }
    public void SetCurrencyQuantity(int currency)
    {
        currencyQuantity = currency;
    }
    public void AddCurrency(int currency)
    {
        currencyQuantity += currency;
    }

    public int GetCupsQuantity(int type)
    {
        return cupStorage[type];
    }

    public int GetTotalCupsQuantity()
    {
        int myTotal = 0;
        foreach (int i in cupStorage)
        {
            myTotal += i;
        }
        return myTotal;
    }
    public void SetCupsQuantity(int cups, int type)
    {
        cupStorage[type] = cups;
    }
    public int TakeNextBestCup()
    {
        int cupSort = 0;
        for(int i = cupStorage.Length-1; i > 0; i--)
        {
            if (cupStorage[i] != 0)
            {
                cupStorage[i] --;
                cupSort = i;
                break;
            }
        }
        return cupSort;
    }
    public void AddCups(int cups, int type)
    {
        cupStorage[type] += cups;
    }

}
