using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class SceneFader : PersistentSingleton<SceneFader>
    {
        [SerializeField] private int transitionDuration;
        [SerializeField] private Image image;
        [SerializeField] private Canvas canvas;
        
        private Material _material;
        private readonly int _fadeAmount = Shader.PropertyToID("_FadeAmount");

        protected override void OnAwake()
        {
            canvas.enabled = false;
            
            Material m = image.material;
            image.material = new Material(m);
            _material = image.material;
        }

        public async Task SwitchScene(string sceneName)
        {
            canvas.enabled = true;

            _material.SetFloat(_fadeAmount, 0);
            
            await _material.DOFloat(1f, _fadeAmount, transitionDuration)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();

            await SceneManager.LoadSceneAsync(sceneName);

            await _material.DOFloat(0f, _fadeAmount, transitionDuration)
                .SetEase(Ease.InOutSine)
                .AsyncWaitForCompletion();

            canvas.enabled = false;
        }


    }
}