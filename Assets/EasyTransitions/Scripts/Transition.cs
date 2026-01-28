using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EasyTransition
{
    public class Transition : MonoBehaviour
    {
        public TransitionSettings transitionSettings;

        public Transform transitionPanelIN;
        public Transform transitionPanelOUT;

        public CanvasScaler transitionCanvas;

        public Material multiplyColorMaterial;
        public Material additiveColorMaterial;

        bool hasTransitionTriggeredOnce;

        private void Start()
        {
            // Keep transition alive across scenes
            DontDestroyOnLoad(gameObject);

            if (transitionCanvas != null && transitionSettings != null)
                transitionCanvas.referenceResolution = transitionSettings.refrenceResolution;

            if (transitionPanelIN != null) transitionPanelIN.gameObject.SetActive(false);
            if (transitionPanelOUT != null) transitionPanelOUT.gameObject.SetActive(false);

            // Setup transition IN
            if (transitionPanelIN != null && transitionSettings.transitionIn != null)
            {
                transitionPanelIN.gameObject.SetActive(true);
                GameObject transitionIn = Instantiate(transitionSettings.transitionIn, transitionPanelIN);
                transitionIn.AddComponent<CanvasGroup>().blocksRaycasts = transitionSettings.blockRaycasts;

                ApplyColorTint(transitionIn);
                ApplyFlipScale(transitionIn.transform);
                ApplyAnimatorSpeed(transitionIn.transform);
            }

            // Cache materials
            multiplyColorMaterial = transitionSettings.multiplyColorMaterial;
            additiveColorMaterial = transitionSettings.addColorMaterial;

            if (multiplyColorMaterial == null || additiveColorMaterial == null)
                Debug.LogWarning("No color tint materials set for transition. Tint changes will not apply.");

            // Subscribe to scene load
            SceneManager.sceneLoaded += OnSceneLoad;
        }

        // 🔑 Made public so TransitionManager can access it
        public void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            if (hasTransitionTriggeredOnce) return;

            if (transitionPanelIN != null) transitionPanelIN.gameObject.SetActive(false);

            if (transitionPanelOUT != null && transitionSettings.transitionOut != null)
            {
                transitionPanelOUT.gameObject.SetActive(true);
                GameObject transitionOut = Instantiate(transitionSettings.transitionOut, transitionPanelOUT);
                transitionOut.AddComponent<CanvasGroup>().blocksRaycasts = transitionSettings.blockRaycasts;

                ApplyColorTint(transitionOut);
                ApplyFlipScale(transitionOut.transform);
                ApplyAnimatorSpeed(transitionOut.transform);
            }

            hasTransitionTriggeredOnce = true;

            float destroyTime = transitionSettings.destroyTime;
            if (transitionSettings.autoAdjustTransitionTime && transitionSettings.transitionSpeed > 0)
                destroyTime = destroyTime / transitionSettings.transitionSpeed;

            Destroy(gameObject, destroyTime);
        }

        private void ApplyColorTint(GameObject transitionObj)
        {
            if (transitionSettings.isCutoutTransition) return;

            if (transitionObj.TryGetComponent<Image>(out Image parentImage))
                ApplyTintToImage(parentImage);

            for (int i = 0; i < transitionObj.transform.childCount; i++)
            {
                if (transitionObj.transform.GetChild(i).TryGetComponent<Image>(out Image childImage))
                    ApplyTintToImage(childImage);
            }
        }

        private void ApplyTintToImage(Image img)
        {
            if (transitionSettings.colorTintMode == ColorTintMode.Multiply && multiplyColorMaterial != null)
            {
                img.material = multiplyColorMaterial;
                img.material.SetColor("_Color", transitionSettings.colorTint);
            }
            else if (transitionSettings.colorTintMode == ColorTintMode.Add && additiveColorMaterial != null)
            {
                img.material = additiveColorMaterial;
                img.material.SetColor("_Color", transitionSettings.colorTint);
            }
        }

        private void ApplyFlipScale(Transform t)
        {
            if (transitionSettings.flipX)
                t.localScale = new Vector3(-t.localScale.x, t.localScale.y, t.localScale.z);
            if (transitionSettings.flipY)
                t.localScale = new Vector3(t.localScale.x, -t.localScale.y, t.localScale.z);
        }

        private void ApplyAnimatorSpeed(Transform t)
        {
            if (transitionSettings.transitionSpeed == 0) return;

            if (t.TryGetComponent<Animator>(out Animator parentAnim))
            {
                parentAnim.speed = transitionSettings.transitionSpeed;
            }
            else
            {
                for (int c = 0; c < t.childCount; c++)
                {
                    if (t.GetChild(c).TryGetComponent<Animator>(out Animator childAnim))
                        childAnim.speed = transitionSettings.transitionSpeed;
                }
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoad;
        }
    }
}
