using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Il2CppScheduleOne.ItemFramework;
using Il2CppScheduleOne.Product;
using Il2CppScheduleOne.StationFramework;
using Il2CppScheduleOne.UI.Phone.ProductManagerApp;
using Il2CppScheduleOne.UI.Tooltips;
using MelonLoader;
using UnityEngine;
using UnityEngine.UI;
using Sys = System.Collections.Generic;
using Il2CppGeneric = Il2CppSystem.Collections.Generic;

[assembly: MelonAuthorColor(1, 255, 0, 255)]
[assembly: MelonColor(255, 255, 130, 30)]
[assembly: System.Reflection.AssemblyMetadata("NexusModID", "1405")]
[assembly: MelonInfo(typeof(ExpandRecipe.Main), "ExpandRecipe-Improved", "1.0.1", "robbmanes, Patched by: MethodNotAllowed")]
[assembly: MelonGame("TVGS", "Schedule I")]

namespace ExpandRecipe
{
	// Token: 0x02000004 RID: 4
	public class Main : MelonMod
	{
		// Token: 0x06000003 RID: 3 RVA: 0x00002067 File Offset: 0x00000267
		public override void OnInitializeMelon()
		{
			MelonLogger.Msg("Tested on Schedule I version \"" + this.testedVersion + "\"");
		}

		// Token: 0x06000004 RID: 4 RVA: 0x00002084 File Offset: 0x00000284
		public override void OnSceneWasLoaded(int buildIndex, string sceneName)
		{
			if (sceneName == "Main")
			{
				try
				{
					Main.productManager = UnityEngine.Object.FindObjectsOfType<ProductManager>()[0];
				}
				catch (Exception value)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Failed to find base Product Manager: ");
					defaultInterpolatedStringHandler.AppendFormatted<Exception>(value);
					MelonLogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			base.OnSceneWasLoaded(buildIndex, sceneName);
		}

		// Token: 0x06000005 RID: 5 RVA: 0x000020F8 File Offset: 0x000002F8
		public static Sys.List<Sys.List<Main.ExpandedItem>> GetExpandedRecipes(ProductEntry productEntry)
		{
			Il2CppGeneric.List<StationRecipe> recipes = productEntry.Definition.Recipes;
			return Main.GetExpandedRecipesInternal(recipes);
		}

		// Token: 0x06000006 RID: 6 RVA: 0x00002110 File Offset: 0x00000310
		private static Sys.List<Sys.List<Main.ExpandedItem>> GetExpandedRecipesInternal(Il2CppGeneric.List<StationRecipe> baseRecipes)
		{
			Sys.List<Sys.List<Main.ExpandedItem>> list = new Sys.List<Sys.List<Main.ExpandedItem>>();
			if (baseRecipes.Count > 0)
			{
				foreach (StationRecipe stationRecipe in baseRecipes)
				{
					Sys.List<StationRecipe.IngredientQuantity> list2 = new Sys.List<StationRecipe.IngredientQuantity>();
					foreach (StationRecipe.IngredientQuantity ingredientQuantity in stationRecipe.Ingredients)
					{
						if (ingredientQuantity.Item.Category == EItemCategory.Product)
						{
							list2 = list2.Append(ingredientQuantity).ToList();
						}
						else
						{
							list2 = list2.Prepend(ingredientQuantity).ToList();
						}
					}
					using (Sys.List<StationRecipe.IngredientQuantity>.Enumerator enumerator3 = list2.GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							StationRecipe.IngredientQuantity ingredient = enumerator3.Current;
							if (ingredient.Item.Category == EItemCategory.Product)
							{
								Func<ProductDefinition, bool> func = (ProductDefinition x) => x.ID == ingredient.Item.ID;
								ProductDefinition productDefinition = Main.productManager.AllProducts.Find(func);
								if (productDefinition == null)
								{
									throw new Exception("Could not find base product for \"'" + ingredient.Item.Name + "'\"");
								}
								ProductDefinition productDefinition2 = productDefinition;
								
								if (productDefinition2.Recipes.Count <= 0)
								{
									Sys.List<Main.ExpandedItem> list3 = new Sys.List<Main.ExpandedItem>();
									list3.Add(new Main.ExpandedItem
									{
										Original = ingredient,
										EffectiveQuantity = (float)ingredient.Quantity
									});
									foreach (StationRecipe.IngredientQuantity ingredientQuantity2 in list2)
									{
										if (ingredientQuantity2 != ingredient)
										{
											list3.Add(new Main.ExpandedItem
											{
												Original = ingredientQuantity2,
												EffectiveQuantity = (float)ingredientQuantity2.Quantity
											});
										}
									}
									list.Add(list3);
								}
								else
								{
									foreach (StationRecipe stationRecipe2 in productDefinition2.Recipes)
									{
										// Cycle Detection: Check if the next recipe loops back to the current ingredient
										bool cycleDetected = false;
										foreach (StationRecipe.IngredientQuantity subIng in stationRecipe2.Ingredients)
										{
											if (subIng.Item.ID == ingredient.Item.ID)
											{
												cycleDetected = true;
												break;
											}
										}
										
										if (cycleDetected)
										{
											// If cycle detected, treat as leaf node to prevent infinite recursion
											Sys.List<Main.ExpandedItem> loopLeafList = new Sys.List<Main.ExpandedItem>();
											loopLeafList.Add(new Main.ExpandedItem
											{
												Original = ingredient,
												EffectiveQuantity = (float)ingredient.Quantity
											});
											
											// Add other ingredients from the current path (list2) minus the current one
											foreach (StationRecipe.IngredientQuantity otherIng in list2)
											{
												if (otherIng != ingredient)
												{
													loopLeafList.Add(new Main.ExpandedItem
													{
														Original = otherIng,
														EffectiveQuantity = (float)otherIng.Quantity
													});
												}
											}
											list.Add(loopLeafList);
											continue; // Skip recursion for this cyclic branch
										}

										float num = (float)stationRecipe2.Product.Quantity;
										float num2 = 1f / num;
										float num3 = (float)ingredient.Quantity;
										float num4 = num2 * num3;
										Il2CppGeneric.List<StationRecipe> list4 = new Il2CppGeneric.List<StationRecipe>();
										list4.Add(stationRecipe2);
										foreach (Sys.List<Main.ExpandedItem> list5 in Main.GetExpandedRecipesInternal(list4))
										{
											Sys.List<Main.ExpandedItem> list6 = new Sys.List<Main.ExpandedItem>();
											foreach (Main.ExpandedItem expandedItem in list5)
											{
												list6.Add(new Main.ExpandedItem
												{
													Original = expandedItem.Original,
													EffectiveQuantity = expandedItem.EffectiveQuantity * num4
												});
											}
											foreach (StationRecipe.IngredientQuantity ingredientQuantity3 in list2)
											{
												if (ingredientQuantity3 != ingredient)
												{
													list6.Add(new Main.ExpandedItem
													{
														Original = ingredientQuantity3,
														EffectiveQuantity = (float)ingredientQuantity3.Quantity
													});
												}
											}
											list.Add(list6);
										}
									}
								}
							}
							else
							{
								bool flag = false;
								using (Sys.List<StationRecipe.IngredientQuantity>.Enumerator enumerator4 = list2.GetEnumerator())
								{
									while (enumerator4.MoveNext())
									{
										if (enumerator4.Current.Item.Category == EItemCategory.Product)
										{
											flag = true;
										}
									}
								}
								if (!flag && list2.IndexOf(ingredient) == 0)
								{
									Sys.List<Main.ExpandedItem> list7 = new Sys.List<Main.ExpandedItem>();
									foreach (StationRecipe.IngredientQuantity ingredientQuantity4 in list2)
									{
										list7.Add(new Main.ExpandedItem
										{
											Original = ingredientQuantity4,
											EffectiveQuantity = (float)ingredientQuantity4.Quantity
										});
									}
									list.Add(list7);
								}
							}
						}
					}
				}
			}
			return list;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000025C4 File Offset: 0x000007C4
		public static GameObject CreateExpandedRecipesTextUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject)
		{
			Transform transform = parentGameObject.transform.Find("ExpandedRecipesText");
			GameObject gameObject;
			if (transform == null)
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
				gameObject.name = "ExpandedRecipesText";
				gameObject.GetComponent<Text>().text = "Expanded Recipe(s)";
			}
			else
			{
				gameObject = transform.gameObject;
			}
			gameObject.gameObject.SetActive(true);
			return gameObject;
		}

		// Token: 0x06000008 RID: 8 RVA: 0x00002630 File Offset: 0x00000830
		public static GameObject GetOrCreateExpandedRecipesUIGameObject(GameObject parentGameObject)
		{
			Transform transform = parentGameObject.transform.Find("ExpandedRecipes");
			GameObject gameObject;
			if (transform == null)
			{
				gameObject = UnityEngine.Object.Instantiate<GameObject>(new GameObject(), parentGameObject.transform).gameObject;
				gameObject.name = "ExpandedRecipes";
				VerticalLayoutGroup verticalLayoutGroup = gameObject.gameObject.AddComponent<VerticalLayoutGroup>();
				verticalLayoutGroup.spacing = 8f;
				verticalLayoutGroup.childScaleHeight = false;
				verticalLayoutGroup.childScaleWidth = false;
				verticalLayoutGroup.childControlHeight = false;
				verticalLayoutGroup.childControlWidth = false;
				verticalLayoutGroup.childForceExpandHeight = false;
				verticalLayoutGroup.childForceExpandWidth = false;
			}
			else
			{
				gameObject = transform.gameObject;
				gameObject.DestroyChildren();
			}
			gameObject.gameObject.SetActive(true);
			return gameObject;
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000026D4 File Offset: 0x000008D4
		public static GameObject CreateExpandedRecipeUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
			gameObject.gameObject.SetActive(true);
			HorizontalLayoutGroup horizontalLayoutGroup = gameObject.gameObject.AddComponent<HorizontalLayoutGroup>();
			horizontalLayoutGroup.childAlignment = (TextAnchor)4;
			horizontalLayoutGroup.childScaleHeight = false;
			horizontalLayoutGroup.childScaleWidth = false;
			horizontalLayoutGroup.childControlHeight = false;
			horizontalLayoutGroup.childControlWidth = true;
			horizontalLayoutGroup.childForceExpandHeight = false;
			horizontalLayoutGroup.childForceExpandWidth = false;
			gameObject.DestroyChildren();
			return gameObject;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x00002740 File Offset: 0x00000940
		public static GameObject CreateBaseProductUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject, Sys.List<Main.ExpandedItem> expandedRecipe)
		{
			Main.ExpandedItem expandedItem = expandedRecipe.First<Main.ExpandedItem>();
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
			gameObject.GetComponent<Image>().sprite = expandedItem.Original.Item.Icon;
			gameObject.GetComponent<Image>().preserveAspect = true;
			Tooltip component = gameObject.GetComponent<Tooltip>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler.AppendFormatted(expandedItem.Original.Item.name);
			defaultInterpolatedStringHandler.AppendLiteral(" (x");
			defaultInterpolatedStringHandler.AppendFormatted<float>(expandedItem.EffectiveQuantity, "0.##");
			defaultInterpolatedStringHandler.AppendLiteral(")");
			component.text = defaultInterpolatedStringHandler.ToStringAndClear();
			return gameObject;
		}

		// Token: 0x0600000B RID: 11 RVA: 0x000027E8 File Offset: 0x000009E8
		public static GameObject CreatePlusUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
			gameObject.GetComponent<Image>().preserveAspect = true;
			return gameObject;
		}

		// Token: 0x0600000C RID: 12 RVA: 0x00002808 File Offset: 0x00000A08
		public static GameObject CreateMixUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject, Main.ExpandedItem ingredient)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
			gameObject.GetComponent<Image>().sprite = ingredient.Original.Item.Icon;
			gameObject.GetComponent<Image>().preserveAspect = true;
			Tooltip component = gameObject.GetComponent<Tooltip>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
			defaultInterpolatedStringHandler.AppendFormatted(ingredient.Original.Item.name);
			defaultInterpolatedStringHandler.AppendLiteral(" (x");
			defaultInterpolatedStringHandler.AppendFormatted<float>(ingredient.EffectiveQuantity, "0.##");
			defaultInterpolatedStringHandler.AppendLiteral(")");
			component.text = defaultInterpolatedStringHandler.ToStringAndClear();
			return gameObject;
		}

		// Token: 0x0600000D RID: 13 RVA: 0x000028A9 File Offset: 0x00000AA9
		public static GameObject CreateArrowUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject)
		{
			return UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x000028BC File Offset: 0x00000ABC
		public static GameObject CreateOutputUIGameObject(GameObject gameObjectToClone, GameObject parentGameObject, StationRecipe.ItemQuantity finalProduct)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObjectToClone, parentGameObject.transform).gameObject;
			gameObject.GetComponent<Image>().sprite = finalProduct.Item.Icon;
			gameObject.GetComponent<Image>().preserveAspect = true;
			gameObject.GetComponent<Tooltip>().text = finalProduct.Item.name;
			return gameObject;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x00002914 File Offset: 0x00000B14
		public static void BuildUIWithRecipe(ProductEntry productEntry, Sys.List<Sys.List<Main.ExpandedItem>> expandedRecipes, GameObject recipeTextUI, GameObject recipesContainerUI)
		{
			if (productEntry.Definition.Recipes == null || productEntry.Definition.Recipes.Count == 0)
			{
				return;
			}
			StationRecipe.ItemQuantity product = productEntry.Definition.Recipes[0].Product;
			GameObject gameObject = recipesContainerUI.transform.Find("Recipe").gameObject;
			if (gameObject == null)
			{
				throw new Exception("Unable to find recipeUI GameObject");
			}
			GameObject gameObject2 = gameObject;
			GameObject gameObject3 = gameObject2.transform.Find("Product").gameObject;
			if (gameObject3 == null)
			{
				throw new Exception("Unable to find productUI GameObject");
			}
			GameObject gameObjectToClone = gameObject3;
			GameObject gameObject4 = gameObject2.transform.Find("Plus").gameObject;
			if (gameObject4 == null)
			{
				throw new Exception("Unable to find plusUI GameObject");
			}
			GameObject gameObjectToClone2 = gameObject4;
			GameObject gameObject5 = gameObject2.transform.Find("Mixer").gameObject;
			if (gameObject5 == null)
			{
				throw new Exception("Unable to find mixerUI GameObject");
			}
			GameObject gameObjectToClone3 = gameObject5;
			GameObject gameObject6 = gameObject2.transform.Find("Arrow").gameObject;
			if (gameObject6 == null)
			{
				throw new Exception("Unable to find arrowUI GameObject");
			}
			GameObject gameObjectToClone4 = gameObject6;
			GameObject gameObject7 = gameObject2.transform.Find("Output").gameObject;
			if (gameObject7 == null)
			{
				throw new Exception("Unable to find outputUI GameObject");
			}
			GameObject gameObjectToClone5 = gameObject7;
			Main.CreateExpandedRecipesTextUIGameObject(recipeTextUI, recipesContainerUI);
			GameObject orCreateExpandedRecipesUIGameObject = Main.GetOrCreateExpandedRecipesUIGameObject(recipesContainerUI);
			Il2CppGeneric.List<StationRecipe> recipes = productEntry.Definition.Recipes;
			if (recipes.Count == expandedRecipes.Count)
			{
				bool flag = true;
				for (int i = 0; i < recipes.Count; i++)
				{
					StationRecipe stationRecipe = recipes[i];
					Sys.List<Main.ExpandedItem> list = expandedRecipes[i];
					if (stationRecipe.Ingredients.Count != list.Count)
					{
						flag = false;
						break;
					}
					foreach (StationRecipe.IngredientQuantity ingredientQuantity in stationRecipe.Ingredients)
					{
						bool flag2 = false;
						using (Sys.List<Main.ExpandedItem>.Enumerator enumerator2 = list.GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								if (enumerator2.Current.Original.Item.ID == ingredientQuantity.Item.ID)
								{
									flag2 = true;
									break;
								}
							}
						}
						if (!flag2)
						{
							flag = false;
							break;
						}
					}
					if (!flag)
					{
						break;
					}
				}
				if (flag)
				{
					if (orCreateExpandedRecipesUIGameObject != null)
					{
						orCreateExpandedRecipesUIGameObject.gameObject.SetActive(false);
					}
					Main.CreateExpandedRecipesTextUIGameObject(recipeTextUI, recipesContainerUI).SetActive(false);
					return;
				}
			}
			foreach (Sys.List<Main.ExpandedItem> list2 in expandedRecipes)
			{
				GameObject parentGameObject = Main.CreateExpandedRecipeUIGameObject(gameObject2, orCreateExpandedRecipesUIGameObject);
				Main.CreateBaseProductUIGameObject(gameObjectToClone, parentGameObject, list2);
				float num = 0f;
				for (int j = 1; j < list2.Count; j++)
				{
					Main.ExpandedItem expandedItem = list2[j];
					if (!(expandedItem.Original.Item == null))
					{
						StorableItemDefinition storableItemDefinition = expandedItem.Original.Item.TryCast<StorableItemDefinition>();
						if (storableItemDefinition != null)
						{
							num += storableItemDefinition.BasePurchasePrice * expandedItem.EffectiveQuantity;
						}
					}
				}
				float value = productEntry.Definition.MarketValue * (float)product.Quantity - num;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 3);
				defaultInterpolatedStringHandler.AppendLiteral("[ExpandRecipe] Product: ");
				defaultInterpolatedStringHandler.AppendFormatted(product.Item.Name);
				defaultInterpolatedStringHandler.AppendLiteral(", Cost: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(num);
				defaultInterpolatedStringHandler.AppendLiteral(", Profit: ");
				defaultInterpolatedStringHandler.AppendFormatted<float>(value);
				MelonLogger.Msg(defaultInterpolatedStringHandler.ToStringAndClear());
				string text = "\n-- Breakdown --";
				for (int k = 1; k < list2.Count; k++)
				{
					Main.ExpandedItem expandedItem2 = list2[k];
					if (!(expandedItem2.Original.Item == null))
					{
						StorableItemDefinition storableItemDefinition2 = expandedItem2.Original.Item.TryCast<StorableItemDefinition>();
						if (storableItemDefinition2 != null)
						{
							float value2 = storableItemDefinition2.BasePurchasePrice * expandedItem2.EffectiveQuantity;
							string str = text;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 4);
							defaultInterpolatedStringHandler2.AppendLiteral("\n");
							defaultInterpolatedStringHandler2.AppendFormatted(expandedItem2.Original.Item.name);
							defaultInterpolatedStringHandler2.AppendLiteral(": $");
							defaultInterpolatedStringHandler2.AppendFormatted<float>(value2, "F2");
							defaultInterpolatedStringHandler2.AppendLiteral(" (x");
							defaultInterpolatedStringHandler2.AppendFormatted<float>(expandedItem2.EffectiveQuantity, "0.##");
							defaultInterpolatedStringHandler2.AppendLiteral(" @ $");
							defaultInterpolatedStringHandler2.AppendFormatted<float>(storableItemDefinition2.BasePurchasePrice);
							defaultInterpolatedStringHandler2.AppendLiteral(")");
							text = str + defaultInterpolatedStringHandler2.ToStringAndClear();
						}
					}
				}
				if (list2.Count > 0)
				{
					list2.Remove(list2[0]);
				}
				int num2 = list2.Count;
				foreach (Main.ExpandedItem ingredient in list2)
				{
					if (num2 >= 0)
					{
						Main.CreatePlusUIGameObject(gameObjectToClone2, parentGameObject);
					}
					Main.CreateMixUIGameObject(gameObjectToClone3, parentGameObject, ingredient);
					num2--;
				}
				Main.CreateArrowUIGameObject(gameObjectToClone4, parentGameObject);
				Tooltip component = Main.CreateOutputUIGameObject(gameObjectToClone5, parentGameObject, product).GetComponent<Tooltip>();
				if (component != null)
				{
					Tooltip tooltip = component;
					string text2 = tooltip.text;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 2);
					defaultInterpolatedStringHandler3.AppendLiteral("\nCost: $");
					defaultInterpolatedStringHandler3.AppendFormatted<float>(num, "F2");
					defaultInterpolatedStringHandler3.AppendLiteral("\nProfit: $");
					defaultInterpolatedStringHandler3.AppendFormatted<float>(value, "F2");
					tooltip.text = text2 + defaultInterpolatedStringHandler3.ToStringAndClear();
					Tooltip tooltip2 = component;
					tooltip2.text += text;
				}
			}
		}

		// Token: 0x04000002 RID: 2
		public string testedVersion = "0.4.2f9";

		// Token: 0x04000003 RID: 3
		public static ProductManager productManager;

		// Token: 0x04000004 RID: 4
		public static ProductManagerApp productManagerApp;

		// Token: 0x02000007 RID: 7
		public struct ExpandedItem
		{
			// Token: 0x04000005 RID: 5
			public StationRecipe.IngredientQuantity Original;

			// Token: 0x04000006 RID: 6
			public float EffectiveQuantity;
		}
	}
}
