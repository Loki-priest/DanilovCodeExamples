using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painter : MonoBehaviour {

    public Camera camera;

    Color col;
    RenderTexture rt;
    Texture2D tex;

    Vector3 relativeHit;
    RaycastHit hit;
    Ray ray;

    bool isEraser = true;
    public bool isActive = true;
    bool canPaint = false;

    ColorItem currentColor;
    public int colorsLeft = 100;
    public List<ToySegment> history;
    ToySegment currentStep;

    public AudioSource soundEraser;
    public AudioSource soundPaint;

    private void Start()
    {
        rt = new RenderTexture(512, 512, 0);
        tex = new Texture2D(512, 512, TextureFormat.ARGB32, false);
    }

    public void SetEraser(bool f)
    {
        isEraser = f;
    }

    public void SetColor(ColorItem ci,Color c)
    {
        canPaint = true;
        if(!isEraser)currentColor = ci; //
        col = c; //currentColor.col;
    }

    public Color GetColor()
    {
        return col;
    }

    public void Undo()
    {
        if (history.Count > 0 && colorsLeft > 0 && !GameController.Instance.gameEnded) //не давать отменять, когда мы закрасили все, а также когда конец игры
        {
            currentStep = history[history.Count - 1];
            currentStep.TxtActive(true);//numTxt.gameObject.SetActive(true);

            currentStep.GetComponent<UI2DSprite>().color = Color.white;
            currentStep.isPainted = false;

            currentColor = currentStep.colorItem;
            if (currentColor)
            {
                currentColor.gameObject.SetActive(true);
                colorsLeft++;
                history.Remove(currentStep);
                UIManager.Instance.ResetColorGrid();
                soundEraser.Play();

                currentColor.ChooseMe();
            }
            //currentColor = null;
        }
    }

    Color ColorMe()
    {
        col.r = Random.Range(0.5f, 1f);
        col.g = Random.Range(0.5f, 1f);
        col.b = Random.Range(0.5f, 1f);
        col.a = 1.0f;

        return col;
    }

    void Update ()
    {
        if (isActive && canPaint && Input.GetMouseButtonUp(0))
        {
            RaycastHit[] hits;
            ray = camera.ScreenPointToRay(Input.mousePosition);
            hits = Physics.RaycastAll(ray);

            for (int i = 0; i < hits.Length; i++)
            {
                hit = hits[i];
                
                relativeHit = hit.transform.worldToLocalMatrix.MultiplyPoint3x4((hit.point - hit.collider.transform.position));

                UI2DSprite ui2d = hit.collider.gameObject.GetComponent<UI2DSprite>();
                ToySegment ts = hit.collider.gameObject.GetComponent<ToySegment>();
                //if (ts) if (ts.isPainted) return; //если наткнулись на покрашенное - игнорить

                if (ui2d && hit.collider.CompareTag("Toy")) //можно добавить, что если не покрашено.
                {
                    int w = ui2d.width;
                    int h = ui2d.height;

                    //копируем копрессированную текстуру в рендертекстуру, чтобы из нее создать некомпрессированную текстуру, чтобы GetPixel был возможен
                    rt.DiscardContents();//rt = new RenderTexture(w, h, 0);//old
                    //RenderTexture.active = rt; //
                    Graphics.Blit(ui2d.sprite2D.texture, rt);
                    //tex = new Texture2D(w, h, TextureFormat.ARGB32, false);//old
                    tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0, false);

                    float scaleX = ui2d.transform.parent.localScale.x;
                    float scaleY = ui2d.transform.parent.localScale.y;
                    //Color c = ui2d.sprite2D.texture.GetPixel((int)(relativeHit.x + w/2.0f), (int)(relativeHit.y + h / 2.0f));
                    Color c = tex.GetPixel((int)(relativeHit.x / scaleX + (float)w / 2.0f), (int)(relativeHit.y / scaleY + (float)h / 2.0f)); //Debug.Log(c); //еще скейл учесть
                    if (c.a > 0.1f) //попали в кусок
                    {
                        //заюзали цвет, спрятать текст
                        ts.TxtActive(isEraser);//numTxt.gameObject.SetActive(isEraser);
                        
                        //спрятать цвет здесь
                        if (isEraser) //стереть, если покрасили когда-то
                        {
                            ui2d.color = GetColor();//ColorMe();//col;
                            ts.isPainted = false;//

                            currentColor = ts.colorItem; //получить цвет текущего куска, чтобы добавить в палитру
                            if (currentColor)
                            {
                                currentColor.gameObject.SetActive(true);
                                colorsLeft++;
                                history.Remove(ts);
                                UIManager.Instance.ResetColorGrid();
                                soundEraser.Play();
                            }
                            currentColor = null; //потратили цвет
                        }
                        else
                        {
                            if (!ts.isPainted) //если кусок не покрашен, то покрасить его в выбранный цвет
                            {
                                ui2d.color = GetColor();//ColorMe();//col;
                                ts.isPainted = true;
                                ts.colorItem = currentColor;
                                currentColor.gameObject.SetActive(false);
                                colorsLeft--;
                                history.Add(ts);
                                UIManager.Instance.ResetColorGrid();
                                soundPaint.Play();
                                canPaint = false;
                            }
                        }

                        if (colorsLeft <= 0)
                        {
                            GameController.Instance.Compare();
                            if (AchieveManager.Instance != null)
                            {
                                AchieveManager.Instance.AddLevel(AchieveManager.TypeAchive.DrawTotalColored, 1);
                            }
                        }

                        return;
                        //canPaint = false; - если раскомментить, то нужно ластик выбирать каждый раз
                    }
                }
            }

        }

	}






}
