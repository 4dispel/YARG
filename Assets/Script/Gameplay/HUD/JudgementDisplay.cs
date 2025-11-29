using TMPro;
using DG.Tweening;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;

namespace YARG.Gameplay.HUD
{
    public class JudgementDisplay : GameplayBehaviour

    {
        [SerializeField]
        private TextMeshProUGUI _judgement;
        [SerializeField]
        private RectTransform _judgementRectTransform;
        [SerializeField]
        private TextMeshProUGUI _timingDetail;
        [SerializeField]
        private Image _earlyLatePanel;

        private CanvasGroup _canvasGroup;

        private Sequence _currentSequence;

        private bool _showingPreview;

        public float FadeIn       = 0.1f;
        public float DisplayTime  = 1.0f;
        public float FadeDuration = 0.5f;

        private int _maxPerfectOffset;

        [SerializeField]
        public TMP_ColorGradient PerfectGradient;
        [SerializeField]
        public Color GreatColor;

        protected override void GameplayAwake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.alpha = 0f;
        }

        public void SetPerfectWindow(double perfectWindow)
        {
            //half the window
            _maxPerfectOffset = (int) math.floor(perfectWindow * 500);
        }

        public void ShowJudgement(int offset)
        {
            //_judgement.text = $"{offset}ms offset";
            if (math.abs(offset) <= _maxPerfectOffset)
            {
                ShowPerfect();
            }
            else
            {
                ShowGreat(offset);
            }

            Animate();
            //Fade();
            //StartCoroutine(FadeCoroutine());
        }

        public void PreviewForEditMode(bool on)
        {
            if (on && _currentSequence == null)
            {
                ShowGreat(1);
                _canvasGroup.alpha = 1f;
                _showingPreview = true;
            }
            else if (!on && _showingPreview)
            {
                _canvasGroup.alpha = 0f;
                _showingPreview = false;
            }
        }

        private void ShowGreat(int offset)
        {
            _judgement.text = "Great";
            _judgement.color = GreatColor;
            _judgement.colorGradientPreset = ScriptableObject.CreateInstance<TMP_ColorGradient>();
            _timingDetail.color = Color.white;
            if (offset > 0)
            {
                //late
                _timingDetail.text = "Late";
                _earlyLatePanel.color = Color.blue;
            }
            else
            {
                //early
                _timingDetail.text = "Early";
                _earlyLatePanel.color = Color.red;
            }
            //Fade();
        }

        private void ShowPerfect()
        {
            _judgement.text = "Perfect";
            _judgement.color = Color.white;
            _judgement.colorGradientPreset = PerfectGradient;
            _earlyLatePanel.color = Color.clear;
            _timingDetail.color = Color.clear;
            //Fade();
        }

        private void Fade()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
            }

            _canvasGroup.alpha = 0f;
            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_canvasGroup.DOFade(1f, FadeIn))
                .AppendInterval(DisplayTime)
                .Append(_canvasGroup.DOFade(0f, FadeDuration))
                .OnComplete((() =>
                {
                    _currentSequence = null;
                }));
        }

        private void Animate()
        {
            if (_currentSequence != null)
            {
                _currentSequence.Kill();
            }

            _canvasGroup.alpha = 1f;
            _judgementRectTransform.localScale = Vector3.one * 0.5f;
            _currentSequence = DOTween.Sequence();
            _currentSequence
                .Append(_judgementRectTransform.DOScale(Vector3.one, FadeIn))
                .AppendInterval(DisplayTime)
                .Append(_canvasGroup.DOFade(0f, FadeDuration))
                .OnComplete((() =>
                {
                    _currentSequence = null;
                }));
        }

    }
}