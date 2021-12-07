using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class randomword : MonoBehaviour
{
    public Text word;
    private int cago;//음식, 과일, 동물, 일상사물
    private string myword;//제시어 저장 공간
    private string [] food = {"짜장면","치킨","피자","떡볶이","파전","산적","족발","샐러드","유부초밥","김밥","라면"
                               ,"사탕","햄버거","계란후라이","파스타","초밥","삼겹살","토스트","만두","핫도그","김치"
                               , "와플","마카롱","케이크","빙수","아이스크림","타코야끼"};
    private string [] fruit = {"사과","복숭아","딸기","배","수박","참외","바나나","망고","용과","귤","포도","파인애플"
                              ,"블루베리","체리","레몬","키위","오렌지","지몽","망고스틴","한라봉","무화과","두리안"};
    private string [] animal = {"코끼리","기린","개","고양이","사자","호랑이","양","고릴라","원숭이","두더지","새","펭귄"
                               ,"여우","늑대","말","토끼","고래","닭","문어","타조","얼룩말","해마","쥐","소","곰","뱀"
                                , "돼지","사슴","고슴도치","부엉이"};
    private string [] someth = {"노트북","연필","스마트폰","의자","바지","안경","액자","이어폰","세탁기","냉장고","휴지통"
                               ,"책","어댑터","창문","손톱깎이","휴지","샴푸","옷걸이","건전지","운동화","변기","욕조","마스크"
                               ,"귀걸이","디퓨저","칫솔","소파","침대","거울","가방","마우스","립밤","향수","청소기","그릇","포크"
                               ,"숟가락","지갑","선풍기"};
   

    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random();
        int ran_cago = random.Next(1, 4);
        switch (ran_cago)
        {
            case 1://음식

                int sele_food = random.Next(0, 26);
                myword = food[sele_food];
                return;

            case 2: //과일

                int sele_fruit = random.Next(0, 21);
                myword = fruit[sele_fruit];
                return;

            case 3: //동물

                int sele_anial = random.Next(0, 29);
                myword = animal[sele_anial];
                return;
            case 4: //일상사물

                int sele_someth = random.Next(0, 38);
                myword = someth[sele_someth];
                return;
            default:
                break;
        }



    }

    // Update is called once per frame
    void Update()
    {
        //내 제시어 UI에 표시
        word.text = "제시어:  " + myword;
    }
}
