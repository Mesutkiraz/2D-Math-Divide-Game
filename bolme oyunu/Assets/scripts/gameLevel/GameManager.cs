using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject karePrefab;
    [SerializeField]
    private Transform karelerpaneli;
    [SerializeField]
    private Transform soruPaneli;
    [SerializeField]
    private Text soruText;

    [SerializeField]
    private Sprite[] kareSprites;
    [SerializeField]
    private GameObject sonucPaneli;

    [SerializeField]
    AudioSource audioSource;

    public AudioClip butonSesi;


    private GameObject[] karelerDizisi = new GameObject[25];

    List<int> bolumDegerleriListesi = new List<int>();
    int bolenSayi, bolunenSayi;
    int kacinciSoru;
    int butonDegeri;
    int dogruSonuc;
    bool butonaBasildidimi;
    int kalanHak;
    string sorununZorlukDerecesi;
    [SerializeField]
    KalanHaklarManager kalanHaklarManager;
    [SerializeField]
    PuanManager puanManager;


    private void Awake()
    {
        sonucPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;
        kalanHak = 3;
        kalanHaklarManager.KalanHaklarýKontolEt(kalanHak);
    }


    void Start()
    {
        butonaBasildidimi = false;
        soruPaneli.GetComponent<RectTransform>().localScale = Vector3.zero;
        kareleriOlustur();
    }

    public void kareleriOlustur()
    {
        for (int i = 0; i < 25; i++)
        {
            GameObject kare = Instantiate(karePrefab, karelerpaneli);
            kare.transform.GetChild(1).GetComponent<Image>().sprite = kareSprites[Random.Range(0, kareSprites.Length)];
            kare.GetComponent<Button>().onClick.AddListener(() => ButonaBasildi());
            karelerDizisi[i] = kare;
        }
        BolumDegerleriniTexteYazdir();
        StartCoroutine(DoFadeRoutine());
        ;
    }
    public void ButonaBasildi()
    {
        if (butonaBasildidimi==true)
        {
            audioSource.PlayOneShot(butonSesi);
            butonDegeri = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text);
            SonucuKontrolEt();
        }
       
    }
    void OyunBitti()
    {
        butonaBasildidimi = false;
        sonucPaneli.GetComponent<RectTransform>().DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    void SonucuKontrolEt()
    {
        if (butonDegeri==dogruSonuc)
        {
            puanManager.PuanArtir(sorununZorlukDerecesi);
            bolumDegerleriListesi.RemoveAt(kacinciSoru);
            if (bolumDegerleriListesi.Count>0)
            {
                soruPaneliniAc();
            }
            else
            {
                OyunBitti();
            }
           
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(1).GetComponent<Image>().enabled=true;
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<Text>().text = "";
            UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetComponent<Button>().interactable = false;
        }
        else
        {
            kalanHak--;
            kalanHaklarManager.KalanHaklarýKontolEt(kalanHak);
        }
        if (kalanHak<=0)
        {
            OyunBitti();
        }
    }

    IEnumerator DoFadeRoutine()
    {
        foreach (var kare in karelerDizisi)
        {
            kare.GetComponent<CanvasGroup>().DOFade(1, 0.2f);
            yield return new WaitForSeconds(0.07f);
        }
        soruPaneliniAc();
    }

    void BolumDegerleriniTexteYazdir()
    {
        foreach(var kare in karelerDizisi)
        {
            int rastgeleDeger = Random.Range(1, 13);
            bolumDegerleriListesi.Add(rastgeleDeger);
            kare.transform.GetChild(0).GetComponent<Text>().text = rastgeleDeger.ToString();
        }
    }
    void soruPaneliniAc()
    {
        soruyuSor();
        butonaBasildidimi = true;
        soruPaneli.GetComponent<RectTransform>().DOScale(1, 0.3f).SetEase(Ease.OutBack);
    }
    void soruyuSor()
    {
        bolenSayi = Random.Range(2, 11);
        kacinciSoru = Random.Range(0, bolumDegerleriListesi.Count);
        dogruSonuc = bolumDegerleriListesi[kacinciSoru];
        bolunenSayi = bolenSayi * dogruSonuc;

        if (bolunenSayi<=40)
        {
            sorununZorlukDerecesi = "kolay";
        }
        else if (bolunenSayi>40 && bolunenSayi<=80)
        {
            sorununZorlukDerecesi = "orta";
        }
        else
        {
            sorununZorlukDerecesi = "zor";
        }
        
        soruText.text = bolunenSayi.ToString() + " : " + bolenSayi.ToString();
    }
}
