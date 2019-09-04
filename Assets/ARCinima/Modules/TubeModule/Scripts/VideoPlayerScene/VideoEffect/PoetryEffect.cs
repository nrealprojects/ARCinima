using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using DG.Tweening;

public class PoetryEffect : MonoBehaviour
{
    [SerializeField]
    SpriteAtlas poetryAtlas;
    [SerializeField]
    GameObject effect;
    SpriteRenderer[] sprites;

    void Start()
    {
        sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].color = new Color(1, 1, 1, 0);
            sprites[i].transform.localScale = Vector3.zero;
        }

        StartCoroutine(PlayeEffect());
    }

    private IEnumerator PlayeEffect()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            GameObject temp = Instantiate(effect);
            temp.transform.position = sprites[i].transform.position;
            sprites[i].DOFade(1, 0.6f);
            sprites[i].transform.DOScale(0.1f * Vector3.one, 0.5f);
            yield return new WaitForSeconds(0.4f);

            if ((i + 1) % 7 == 0)
            {
                yield return new WaitForSeconds(0.8f);
            }
        }
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutAnimator());
    }

    private IEnumerator FadeOutAnimator()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].DOFade(0, 1f);
        }
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}