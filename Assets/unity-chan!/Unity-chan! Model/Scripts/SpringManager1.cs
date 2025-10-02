using UnityEngine;
using System.Collections;

namespace UnityChan
{
	public class SpringManager : MonoBehaviour
	{
		// DynamicRatio is parameter for activated level of dynamic animation 
		public float dynamicRatio = 1.0f;

		public float stiffnessForce;
		public AnimationCurve stiffnessCurve;
		public float dragForce;
		public AnimationCurve dragCurve;

		// 重力のベクトル
		public float gravity = 0.008f;
		public SpringBone[] springBones;

		public bool clothe;
		public bool Breasts;

		void Start()
		{
			if (clothe)
			{
				UpdateParametersClothe();
			}
			else if (Breasts)
			{
				UpdateParametersBreasts();
			}
			else
			{
				UpdateParameters();
			}
		}

		void Update()
		{
#if UNITY_EDITOR
			if (dynamicRatio >= 1.0f)
				dynamicRatio = 1.0f;
			else if (dynamicRatio <= 0.0f)
				dynamicRatio = 0.0f;

			if (clothe)
			{
				UpdateParametersClothe();
			}
			else if (Breasts)
			{
				UpdateParametersBreasts();
			}
			else
			{
				UpdateParameters();
			}
#endif
		}

		private void LateUpdate()
		{
			if (dynamicRatio != 0.0f)
			{
				for (int i = 0; i < springBones.Length; i++)
				{
					if (dynamicRatio > springBones[i].threshold)
					{
						springBones[i].UpdateSpring();
					}
				}
			}
		}

		[ContextMenu("reset bones of the skeleton")]
		public void resetAllBones()
		{
			// 空のTransform配列を用意
			Transform[] transforms = new Transform[0];

			// springBones配列を空にリセット
			springBones = new SpringBone[0];
		}

		[ContextMenu("Obtain bones of the skeleton")]
		public void GetAllBones()
		{
			Transform[] transforms = GetComponentsInChildren<Transform>();
			foreach (Transform t in transforms)
			{
				if (t.name.StartsWith("H") || t.name.StartsWith("臂") || t.name.StartsWith("后裙_") || t.name.StartsWith("前裙_"))
				{
					// 全てのコンポーネントを取得
					Component[] components = t.GetComponents<Component>();
					foreach (Component comp in components)
					{
						// Transformコンポーネント以外を削除
						if (!(comp is Transform))
						{
							DestroyImmediate(comp);
						}
					}

					if (t.childCount > 0)
					{
						t.gameObject.AddComponent<SpringBone>();
						t.gameObject.GetComponent<SpringBone>().child = t.GetChild(0);
						t.gameObject.GetComponent<SpringBone>().boneAxis = new Vector3(0, 1, 0);
					}
				}
			}
			springBones = GetComponentsInChildren<SpringBone>();
		}

		private void UpdateParameters()
		{
			UpdateParameter("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter("dragForce", dragForce, dragCurve);
			UpdateParameterGravity("gravityNum", gravity);
		}

		private void UpdateParametersClothe()
		{
			UpdateParameter("stiffnessForce", stiffnessForce, stiffnessCurve);
			UpdateParameter("dragForce", dragForce, dragCurve);
			UpdateParameterGravity("gravityNum", gravity);
		}

		private void UpdateParametersBreasts()
		{
			UpdateParameterBreasts("stiffnessForce", stiffnessForce);
			UpdateParameterBreasts("dragForce", dragForce);
			UpdateParameterGravity("gravityNum", gravity);
		}

		private void UpdateParameter(string fieldName, float baseValue, AnimationCurve curve)
		{
			var start = curve.keys[0].time;
			var end = curve.keys[curve.length - 1].time;

			var prop = springBones[0].GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

			for (int i = 0; i < springBones.Length; i++)
			{
				if (!springBones[i].isUseEachBoneForceSettings)
				{
					var scale = curve.Evaluate(start + (end - start) * i / (springBones.Length - 1));
					prop.SetValue(springBones[i], baseValue * scale);
				}
			}
		}

		private void UpdateParameterGravity(string fieldName, float baseValue)
		{
			var prop = springBones[0].GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

			for (int i = 0; i < springBones.Length; i++)
			{
				if (!springBones[i].isUseEachBoneForceSettings)
				{
					prop.SetValue(springBones[i], baseValue);
				}
			}
		}
		private void UpdateParameterBreasts(string fieldName, float baseValue)
		{
			var prop = springBones[0].GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

			for (int i = 0; i < springBones.Length; i++)
			{
				if (!springBones[i].isUseEachBoneForceSettings)
				{
					prop.SetValue(springBones[i], baseValue);
				}
			}
		}
	}
}
