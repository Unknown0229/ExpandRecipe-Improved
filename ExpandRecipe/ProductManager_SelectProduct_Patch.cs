using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ExpandRecipe;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.StationFramework;
using Il2CppScheduleOne.UI.Phone.ProductManagerApp;
using UnityEngine;
using HarmonyLib;
using MelonLoader;

namespace ExpandRecipe
{
	// Token: 0x02000006 RID: 6
	[HarmonyPatch(typeof(ProductManagerApp), "SelectProduct")]
	public static class ProductManager_SelectProduct_Patch
	{
		// Token: 0x06000012 RID: 18 RVA: 0x00002F40 File Offset: 0x00001140
		public static void Prefix(ProductManagerApp __instance, ProductEntry entry)
		{
			Main.productManagerApp = __instance;
			Transform transform;
			Transform transform2;
			try
			{
				ProductAppDetailPanel detailPanel = __instance.DetailPanel;
				transform = detailPanel.transform.Find("Scroll View/Viewport/Content/RecipesContainer");
				if (transform == null)
				{
					MelonLogger.Error("Can't find RecipesContainer object in current scene");
					return;
				}
				transform2 = detailPanel.transform.Find("Scroll View/Viewport/Content/Recipes");
				if (transform2 == null)
				{
					MelonLogger.Error("Can't find Recipes object in current scene");
					return;
				}
				Transform transform3 = transform.Find("ExpandedRecipes");
				if (transform3 != null)
				{
					transform3.gameObject.SetActive(false);
				}
				Transform transform4 = transform.Find("ExpandedRecipesText");
				if (transform4 != null)
				{
					transform4.gameObject.SetActive(false);
				}
			}
			catch (Exception value)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler.AppendLiteral("Failed to find Phone UI components: ");
				defaultInterpolatedStringHandler.AppendFormatted<Exception>(value);
				MelonLogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				return;
			}
			List<List<Main.ExpandedItem>> expandedRecipes;
			try
			{
				expandedRecipes = Main.GetExpandedRecipes(entry);
				foreach (List<Main.ExpandedItem> list in expandedRecipes)
				{
					string name = entry.Definition.Name;
					string text = "";
					foreach (Main.ExpandedItem expandedItem in list)
					{
						if (text.Length > 0)
						{
							text += " + ";
						}
						string str = text;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
						defaultInterpolatedStringHandler2.AppendFormatted(expandedItem.Original.Item.name);
						defaultInterpolatedStringHandler2.AppendLiteral("(x");
						defaultInterpolatedStringHandler2.AppendFormatted<float>(expandedItem.EffectiveQuantity, "0.##");
						defaultInterpolatedStringHandler2.AppendLiteral(")");
						text = str + defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					MelonLogger.Msg("Expanded Recipe for \"" + name + "\": " + text);
				}
			}
			catch (Exception value2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("Failed to get expanded recipe list: ");
				defaultInterpolatedStringHandler3.AppendFormatted<Exception>(value2);
				MelonLogger.Error(defaultInterpolatedStringHandler3.ToStringAndClear());
				return;
			}
			try
			{
				Main.BuildUIWithRecipe(entry, expandedRecipes, transform2.gameObject, transform.gameObject);
			}
			catch (Exception value3)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("Exception raised building UI component: ");
				defaultInterpolatedStringHandler4.AppendFormatted<Exception>(value3);
				MelonLogger.Error(defaultInterpolatedStringHandler4.ToStringAndClear());
			}
		}
	}
}
