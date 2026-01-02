using System;
using UnityEngine;

namespace ExpandRecipe
{
	// Token: 0x02000005 RID: 5
	public static class GameObjectExtensions
	{
		// Token: 0x06000011 RID: 17 RVA: 0x00002F04 File Offset: 0x00001104
		public static void DestroyChildren(this GameObject thisGameObject)
		{
			for (int i = thisGameObject.transform.GetChildCount() - 1; i >= 0; i--)
			{
				UnityEngine.Object.Destroy(thisGameObject.transform.GetChild(i).gameObject);
			}
		}
	}
}
