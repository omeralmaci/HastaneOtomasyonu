using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Runtime.InteropServices;

/*  
 *  #######################################################################################################
 *  ##                                                                                                   ##
 *  ##                                    Süleyman Demirel Üniversitesi                                  ##
 *  ##                                        Teknoloji Fakültesi                                        ##
 *  ##                                  Mekatronik Mühendisliği 2.Öğretim                                ##
 *  ##                                                                                                   ##
 *  ##                                  Nesne Yönelimli Programlamaya Giriş                              ##
 *  ##                                                                                                   ##
 *  ##                                      Hastane Otomasyonu Sistemi                                   ##
 *  ##                                                                                                   ##
 *  ##                                                                                                   ##
 *  ##             Ömer Faruk ALMACI                                                                     ##
 *  ##             1522709001                                                                            ##
 *  ##             2.Sınıf / A Şubesi                                     Teslim Tarihi : 12.12.2016     ##
 *  ##                                                          Uzatılmış Teslim Tarihi : 19.12.2016     ##
 *  ####################################################################################################### 
 */

namespace hOtomasyon
{
    //****************************************************** String Numerik Kontrolü *********************
    public static class ExtensionManager
        {
            public static bool IsNumeric(this string text)
            {
                foreach (char chr in text)
                {
                    if (!Char.IsNumber(chr)) return false;

                }
                return true;
            }
        }
    //****************************************************** Program Sınıfı ******************************
    class Program
    {
        //########################################## TAM EKRAN ############################################
        [DllImport("kernel32.dll", ExactSpelling = true)]

        private static extern IntPtr GetConsoleWindow();
        private static IntPtr ThisConsole = GetConsoleWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]

        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int GIZLE = 0;
        private const int BUYULTMEK = 3;
        private const int KUCULTMEK = 6;
        private const int STANDART = 9;
        //───────────────────────────────────────────────────── STATİK DEĞİŞKEN TANIMLAMA ───────────────
        public static OleDbConnection baglanti;
        public static OleDbCommand komut;
        public static OleDbDataReader oku;
        //########################################## ANA MENU ############################################
        public static int MenuListele()
        {
            int menuSecim;
            menusecimdon:
            
            Console.WriteLine("\n");
            Console.WriteLine("\t\t\t\t\t\t\t\t    [1] Hasta İşlemleri");
            Console.WriteLine("\t\t\t\t\t\t\t\t    [2] Randevu İşlemleri");
            Console.WriteLine("\t\t\t\t\t\t\t\t    [3] Muayene İşlemleri");
            Console.WriteLine("\t\t\t\t\t\t\t\t    [4] Doktor İşlemleri\n");
            Console.WriteLine("\t\t\t\t\t\t\t\t    [Ç] Çıkış");
            Console.WriteLine("\t\t\t\t\t\t\t\t    [H] Hakkında");
            
            do
            {
                Console.Write("\t\tSeçiminiz:  ");
                string menusec1 = Console.ReadLine();
                if ((menusec1 == "1") || (menusec1 == "2") || (menusec1 == "3") || (menusec1 == "4") || (menusec1 == "Ç") || (menusec1 == "ç") || (menusec1 == "H") || (menusec1 == "h"))
                {
                    if (menusec1 == "Ç" || menusec1 == "ç")
                    {
                        menusec1 = "5";
                        menuSecim = int.Parse(menusec1);
                    }
                    else if (menusec1 == "h" || menusec1=="H")
                    {
                        menusec1 = "6";
                        menuSecim = int.Parse(menusec1);
                    }
                    else
                    {
                        menuSecim = int.Parse(menusec1);

                    }
                }
                else
                {
                    Console.WriteLine("\tHatalı Giriş Yaptınız, Tekrar Deneyiniz!");
                    goto menusecimdon;
                }
                
                if (!(menuSecim >= 1 && menuSecim <= 6))
                {
                    Console.WriteLine("\tYanlış Seçim Yaptınız, Tekrar Deneyiniz...");
                }
            } while (!((menuSecim >= 1) && (menuSecim <= 6)));
            return menuSecim;
        }
        //########################################## RANDAVU MENUSU ######################################

        //─────────────────────────────────────────────────────────────────────── Randevu KAYIT ──────────        
        static void RandevuKayit()
        {
            tekrar:
            Console.WriteLine("Randevu Vermek İstediğiniz Hastanın TC Kimlik Numarasını Giriniz: ");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
            devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
            devam:
            Console.WriteLine("Randavu Tarihini Giriniz [GG.AA.YYYY]: ");
        tekrarRtarih:
            Console.WriteLine("Hastanın Doğum Tarihini Giriniz: ");
            string tarih = Console.ReadLine();
            int uzunluk = tarih.Length;
            if (uzunluk > 10 || uzunluk < 8)
            {
                Console.WriteLine("Geçersiz Tarih Uzunluğu Girdiniz. Tekrar Deneyiniz...");
                goto tekrarRtarih;
            }
            else
            {
                if (!(tarih.Substring(2, 1) == "." || tarih.Substring(5, 1) == "."))
                {
                    Console.WriteLine("Geçersiz Tarih Formatı Girdiniz. Tekrar Deneyiniz...");
                    goto tekrarRtarih;
                }
                else
                {
                    string gun = tarih.Substring(0, 2);
                    if (Convert.ToInt16(gun) > 31 || Convert.ToInt16(gun) < 1)
                    {
                        Console.WriteLine("Hatalı Gün girdiniz. Tekrar Deneyiniz...");
                        goto tekrarRtarih;

                    }
                    else
                    {
                        string ay = tarih.Substring(3, 2);
                        if (Convert.ToInt16(ay) > 12 || Convert.ToInt16(ay) < 1)
                        {
                            Console.WriteLine("Hatalı Ay Girdiniz. Tekrar Deneyiniz...");
                            goto tekrarRtarih;
                        }
                        else
                        {
                            string yil = tarih.Substring(6, 4);
                            if (Convert.ToInt16(yil) > 3000 || Convert.ToInt16(yil) < 1500)
                            {
                                Console.WriteLine("Hatalı Yıl Girdiniz. Tekrar Deneyiniz...");
                                goto tekrarRtarih;
                            }
                            else
                            {
                                goto devamRtarih;
                            }
                        }
                    }
                }
            }
        devamRtarih:
            DateTime rTarihgir = DateTime.Parse(tarih);
            string rTarih = rTarihgir.ToString("dd.MM.yyy");
            //rTarih.ToShortDateString();
            Console.WriteLine("Randevu Saatini Giriniz [SS:DD]: ");
        tekrarRsaat:
            Console.WriteLine("Hastanın Doğum Tarihini Giriniz: ");
            string saat = Console.ReadLine();
            int uSaat = saat.Length;
            if (uSaat > 5 || uSaat < 4)
            {
                Console.WriteLine("Geçersiz Saat Uzunluğu Girdiniz. Tekrar Deneyiniz...");
                goto tekrarRsaat;
            }
            else
            {
                if (!(saat.Substring(2, 1) == ":"))
                {
                    Console.WriteLine("Geçersiz Tarih Formatı Girdiniz. Tekrar Deneyiniz...");
                    goto tekrarRtarih;
                }
                else
                {
                    string ss = saat.Substring(0, 2);
                    if (Convert.ToInt16(ss) > 24 || Convert.ToInt16(ss) < 0)
                    {
                        Console.WriteLine("Hatalı Saat girdiniz. Tekrar Deneyiniz...");
                        goto tekrarRtarih;

                    }
                    else
                    {
                        string dd = saat.Substring(3, 2);
                        if (Convert.ToInt16(dd) > 60 || Convert.ToInt16(dd) < 0)
                        {
                            Console.WriteLine("Hatalı Dakika Girdiniz. Tekrar Deneyiniz...");
                            goto tekrarRtarih;
                        }
                        else
                        {
                            goto devamRsaat;
                            
                        }
                    }
                }
            }
        devamRsaat:
            DateTime rSaatgir = DateTime.Parse(saat);
            string rSaat = rSaatgir.ToString("HH:m");
        // rSaat.ToShortTimeString();
        poligeri:
            Console.WriteLine("Hastanın Giriş Yapmak İstediği Polikliniği Giriniz: ");
            String poliklinik;
            String poli = Console.ReadLine();
            if (poli.Length <= 15)
            {
                poliklinik = poli;
            }
            else
            {
                Console.WriteLine("Lütfen 16 Karakterden Daha Kısa Metin Giriniz.");
                goto poligeri;
            }
            dokgeri:
            Console.WriteLine("Hastayı Muayene Edecek Doktorun İsmini Giriniz: ");
            String doktorAdi;
            String dok_Adi = Console.ReadLine();
            if (dok_Adi.Length <= 11)
            {
                doktorAdi = dok_Adi;
            }
            else
            {
                Console.WriteLine("Lütfen 12 Karakterden Daha Kısa Metin Giriniz.");
                goto dokgeri;
            }
            sikageri:
            Console.WriteLine("Hastanın Şikayetlerini Giriniz");
            String sikayetler;
            String s_kontrol = Console.ReadLine();
            if (s_kontrol.Length <= 12)
            {
                sikayetler = s_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 13 Karakterden Kısa Metin Giriniz");
                goto sikageri;
            }
            string r_Kontrol = "R_Alindi";

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hRandevu Set r_Kontrol=@r_Kontrol, rTarih=@rTarih, rSaat=@rSaat, poliklinik=@polikilik, doktorAdi=@doktorAdi,sikayetler=@sikayetler Where tcNo=@tcNo";
            
            komut.Parameters.AddWithValue("@r_Kontrol", r_Kontrol);
            komut.Parameters.AddWithValue("@rTarih", rTarih);
            komut.Parameters.AddWithValue("@rSaat", rSaat.ToString());
            komut.Parameters.AddWithValue("@poliklinik", poliklinik);
            komut.Parameters.AddWithValue("@doktorAdi", doktorAdi);
            komut.Parameters.AddWithValue("@sikayetler", sikayetler);
            komut.Parameters.AddWithValue("@tcNo", tcNo);

            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n Kayıt Başarıyla Eklendi");
            }
            else
            {
                Console.WriteLine("\n Kayıt Başarısız");
            }
        }
        //─────────────────────────────────────────────────────────────────────── Randevu Güncelleme ─────
        static void RandevuGuncelle()
        {          
        tekrar:
            Console.WriteLine("Güncellemek İstediğiniz Hastanın TC Kimlik Numarasını Giriniz: ");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
            Console.WriteLine("Randavu Tarihini Giriniz [GG.AA.YYYY]: ");
        tekrarRtarih:
            Console.WriteLine("Hastanın Doğum Tarihini Giriniz: ");
            string tarih = Console.ReadLine();
            int uzunluk = tarih.Length;
            if (uzunluk > 10 || uzunluk < 8)
            {
                Console.WriteLine("Geçersiz Tarih Uzunluğu Girdiniz. Tekrar Deneyiniz...");
                goto tekrarRtarih;
            }
            else
            {
                if (!(tarih.Substring(2, 1) == "." || tarih.Substring(5, 1) == "."))
                {
                    Console.WriteLine("Geçersiz Tarih Formatı Girdiniz. Tekrar Deneyiniz...");
                    goto tekrarRtarih;
                }
                else
                {
                    string gun = tarih.Substring(0, 2);
                    if (Convert.ToInt16(gun) > 31 || Convert.ToInt16(gun) < 1)
                    {
                        Console.WriteLine("Hatalı Gün girdiniz. Tekrar Deneyiniz...");
                        goto tekrarRtarih;

                    }
                    else
                    {
                        string ay = tarih.Substring(3, 2);
                        if (Convert.ToInt16(ay) > 12 || Convert.ToInt16(ay) < 1)
                        {
                            Console.WriteLine("Hatalı Ay Girdiniz. Tekrar Deneyiniz...");
                            goto tekrarRtarih;
                        }
                        else
                        {
                            string yil = tarih.Substring(6, 4);
                            if (Convert.ToInt16(yil) > 3000 || Convert.ToInt16(yil) < 1500)
                            {
                                Console.WriteLine("Hatalı Yıl Girdiniz. Tekrar Deneyiniz...");
                                goto tekrarRtarih;
                            }
                            else
                            {
                                goto devamRtarih;
                            }
                        }
                    }
                }
            }
        devamRtarih:
            DateTime rTarihgir = DateTime.Parse(tarih);
            string rTarih = rTarihgir.ToString("dd.MM.yyy");
            //rTarih.ToShortDateString();
            Console.WriteLine("Randevu Saatini Giriniz [SS:DD]: ");
        tekrarRsaat:
            Console.WriteLine("Hastanın Doğum Tarihini Giriniz: ");
            string saat = Console.ReadLine();
            int uSaat = saat.Length;
            if (uSaat > 5 || uSaat < 4)
            {
                Console.WriteLine("Geçersiz Saat Uzunluğu Girdiniz. Tekrar Deneyiniz...");
                goto tekrarRsaat;
            }
            else
            {
                if (!(saat.Substring(2, 1) == ":"))
                {
                    Console.WriteLine("Geçersiz Tarih Formatı Girdiniz. Tekrar Deneyiniz...");
                    goto tekrarRtarih;
                }
                else
                {
                    string ss = saat.Substring(0, 2);
                    if (Convert.ToInt16(ss) > 24 || Convert.ToInt16(ss) < 0)
                    {
                        Console.WriteLine("Hatalı Saat girdiniz. Tekrar Deneyiniz...");
                        goto tekrarRtarih;

                    }
                    else
                    {
                        string dd = saat.Substring(3, 2);
                        if (Convert.ToInt16(dd) > 60 || Convert.ToInt16(dd) < 0)
                        {
                            Console.WriteLine("Hatalı Dakika Girdiniz. Tekrar Deneyiniz...");
                            goto tekrarRtarih;
                        }
                        else
                        {
                            goto devamRsaat;

                        }
                    }
                }
            }
        devamRsaat:
            DateTime rSaatgir = DateTime.Parse(saat);
            string rSaat = rSaatgir.ToString("HH:m");
        // rSaat.ToShortTimeString();
            poligeri:
            Console.WriteLine("Hastanın Giriş Yapmak İstediği Polikliniği Giriniz: ");
            String poliklinik;
            String poli = Console.ReadLine();
            if (poli.Length <= 15)
            {
                poliklinik = poli;
            }
            else
            {
                Console.WriteLine("Lütfen 16 Karakterden Daha Kısa Metin Giriniz.");
                goto poligeri;
            }
             dokgeri:
            Console.WriteLine("Hastayı Muayene Edecek Doktorun İsmini Giriniz: ");
            String doktorAdi;
            String dok_Adi = Console.ReadLine();
            if (dok_Adi.Length <= 11)
            {
                doktorAdi = dok_Adi;
            }
            else
            {
                Console.WriteLine("Lütfen 12 Karakterden Daha Kısa Metin Giriniz.");
                goto dokgeri;
            }
            sikageri:
            Console.WriteLine("Hastanın Şikayetlerini Giriniz");
            String sikayetler;
            String s_kontrol = Console.ReadLine();
            if (s_kontrol.Length <= 12)
            {
                sikayetler = s_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 13 Karakterden Kısa Metin Giriniz");
                goto sikageri;
            }

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hOtomasyon Set rTarih=@rTarih, rSaat=@rSaat, poliklinik=@polikilik, doktorAdi=@doktorAdi,sikayetler=@sikayetler Where tcNo=@tcNo";

            
            komut.Parameters.AddWithValue("@rTarih", rTarih);
            komut.Parameters.AddWithValue("@rSaat", rSaat.ToString());
            komut.Parameters.AddWithValue("@poliklinik", poliklinik);
            komut.Parameters.AddWithValue("@doktorAdi", doktorAdi);
            komut.Parameters.AddWithValue("@sikayetler", sikayetler);
            komut.Parameters.AddWithValue("@tcNo", tcNo);

            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n Kayıt Başarıyla Eklendi");
            }
            else
            {
                Console.WriteLine("\n Kayıt Başarısız");
            }
        }
        //─────────────────────────────────────────────────────────────────────── Randevu SIL ────────────
        static void RandevuSil()
        {
        tekrar:
            Console.WriteLine("Randevusunu Silmek İstediğiniz Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
            string rTarih = "";
            string rSaat = "";
            String poliklinik = "";
            String doktorAdi = "";
            String sikayetler = "";
            String r_Kontrol = ""; 

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hOtomasyon Set r_Kontrol=@r_Kontrol, rTarih=@rTarih, rSaat=@rSaat, poliklinik=@polikilik, doktorAdi=@doktorAdi,sikayetler=@sikayetler Where tcNo=@tcNo";


            komut.Parameters.AddWithValue("@r_Kontrol", r_Kontrol);
            komut.Parameters.AddWithValue("@rTarih", rTarih);
            komut.Parameters.AddWithValue("@rSaat", rSaat.ToString());
            komut.Parameters.AddWithValue("@poliklinik", poliklinik);
            komut.Parameters.AddWithValue("@doktorAdi", doktorAdi);
            komut.Parameters.AddWithValue("@sikayetler", sikayetler);
            komut.Parameters.AddWithValue("@tcNo", tcNo);

            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n"+tcNo+" Tc Kimlik Numaralı Randevu Başarı İle Silindi.");
            }
            else
            {
                Console.WriteLine("\n Silme İşlemi Başarısız.");
            }
        }
        //########################################## Muayene Menusu ######################################
        //─────────────────────────────────────────────────────────────────────── Muayene Kayit ──────────
        static void MuayeneKayit()
        {
        tekrar:
            Console.WriteLine("Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
            tesgeri:
            Console.WriteLine("Hastanın Hastalık Teşhisi: ");
            String teshis;
            String teshis_kontrol = Console.ReadLine();
            if (teshis_kontrol.Length <= 12)
            {
                teshis = teshis_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 13 Karakterden Kısa Metin Giriniz.");
                goto tesgeri;
            }
            ilacgeri:
            Console.WriteLine("Hastanın Kullanacağı İlaçlar: ");
            String ilaclar;
            String ilac_kontrol = Console.ReadLine();
            if (ilac_kontrol.Length <= 9)
            {
                ilaclar = ilac_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 10 Karakterden Kısa Metin Giriniz.");
                goto ilacgeri;
            }
            tahlilgeri:
            Console.WriteLine("Hastanın Tahlil Sonuçları: ");
            String tahliller;
            String tahlil_kontrol = Console.ReadLine();
            if (tahlil_kontrol.Length <= 8)
            {
                tahliller = tahlil_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 9 Karakterden Kısa Metin Giriniz.");
                goto tahlilgeri;
            }

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hOtomasyon Set teshis=@teshis, ilaclar=@ilaclar, tahliller=@tahliller Where tcNo=@tcNo";


            komut.Parameters.AddWithValue("@teshis", teshis);
            komut.Parameters.AddWithValue("@ilaclar", ilaclar);
            komut.Parameters.AddWithValue("@tahliller", tahliller);
            komut.Parameters.AddWithValue("@tcNo", tcNo);

            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n" + tcNo + " Tc Kimlik Numaralı Hastanın Muayene Kaydı Başarı ile Oluşturuldu.");
            }
            else
            {
                Console.WriteLine("\n Kayıt İşlemi Başarısız.");
            }
        }
        //─────────────────────────────────────────────────────────────────────── Muayene Güncelleme ─────
        static void MuayeneGuncelle()
        {
        tekrar:
            Console.WriteLine("Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
        tesgeri:
            Console.WriteLine("Hastanın Hastalık Teşhisi: ");
            String teshis;
            String teshis_kontrol = Console.ReadLine();
            if (teshis_kontrol.Length <= 12)
            {
                teshis = teshis_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 13 Karakterden Kısa Metin Giriniz.");
                goto tesgeri;
            }
        ilacgeri:
            Console.WriteLine("Hastanın Kullanacağı İlaçlar: ");
            String ilaclar;
            String ilac_kontrol = Console.ReadLine();
            if (ilac_kontrol.Length <= 9)
            {
                ilaclar = ilac_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 10 Karakterden Kısa Metin Giriniz.");
                goto ilacgeri;
            }
        tahlilgeri:
            Console.WriteLine("Hastanın Tahlil Sonuçları: ");
            String tahliller;
            String tahlil_kontrol = Console.ReadLine();
            if (tahlil_kontrol.Length <= 8)
            {
                tahliller = tahlil_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 9 Karakterden Kısa Metin Giriniz.");
                goto tahlilgeri;
            }

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hOtomasyon Set teshis=@teshis, ilaclar=@ilaclar, tahliller=@tahliller Where tcNo=@tcNo";


            komut.Parameters.AddWithValue("@teshis", teshis);
            komut.Parameters.AddWithValue("@ilaclar", ilaclar);
            komut.Parameters.AddWithValue("@tahliller", tahliller);
            komut.Parameters.AddWithValue("@tcNo", tcNo);

            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n" + tcNo + " Tc Kimlik Numaralı Hastanın Muayene Kaydı Başarı ile Oluşturuldu.");
            }
            else
            {
                Console.WriteLine("\n Kayıt İşlemi Başarısız.");
            }
        }
        //─────────────────────────────────────────────────────────────────────── Muayene Kayıt Silme ────
        static void MuayeneSil()
        {
        tekrar:
            Console.WriteLine("Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
            String teshis = "";
            string ilaclar = "";
            string tahliller = "";

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hOtomasyon Set teshis=@teshis, ilaclar=@ilaclar, tahliller=@tahliller Where tcNo=@tcNo";


            komut.Parameters.AddWithValue("@teshis", teshis);
            komut.Parameters.AddWithValue("@ilaclar", ilaclar);
            komut.Parameters.AddWithValue("@tahliller", tahliller);
            komut.Parameters.AddWithValue("@tcNo", tcNo);

            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n" + tcNo + " Tc Kimlik Numaralı Hastanın Muayene Kaydı Başarı ile Silinmiştir.");
            }
            else
            {
                Console.WriteLine("\n Silme İşlemi Başarısız.");
            }
        }
        //########################################## Hasta Menusu ########################################
        //─────────────────────────────────────────────────────────────────────── Hasta Kayıt ────────────
        public static void Kayit()
        {
        tekrar:
            Console.WriteLine("Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
            adgeri:
            Console.WriteLine("Hastanın Adını Giriniz: ");
            String ad;
            String ad_kontrol = Console.ReadLine();
            if (ad_kontrol.Length <= 10)
            {
                ad = ad_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 11 Karakterden Kısa Metin Giriniz");
                goto adgeri;
            }
            soyadgeri:
            Console.WriteLine("Hastanın Soyadını Giriniz");
            String soyAd;
            String soyad_kontrol = Console.ReadLine();
            if (soyad_kontrol.Length<=9)
            {
                soyAd = soyad_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 10 Karakterden Kısa Metin Giriniz");
                goto soyadgeri;
            }
            Console.WriteLine("Hastanın Cinsiyetini Giriniz Erkek[E] - Kadın[K]");
            string cinsiyet = Console.ReadLine();
            if ((cinsiyet=="E") || (cinsiyet=="e"))
            {
                cinsiyet = "Erkek";
            }
            else if((cinsiyet=="K")||(cinsiyet=="k"))
            {
                cinsiyet = "Kadın";
            }
            else
            {
                Console.WriteLine("Lütfen Geçerli Cinsiyet Giriniz!");
            }
            tekrardtarih:
            Console.WriteLine("Hastanın Doğum Tarihini Giriniz: ");
            string tarih = Console.ReadLine();
            int uzunluk = tarih.Length;
            if (uzunluk > 10 || uzunluk < 8)
            {
                Console.WriteLine("Geçersiz Tarih Uzunluğu Girdiniz. Tekrar Deneyiniz...");
                goto tekrardtarih;
            }
            else
            {
                if (!(tarih.Substring(2, 1) == "." || tarih.Substring(5, 1) == "."))
                {
                    Console.WriteLine("Geçersiz Tarih Formatı Girdiniz. Tekrar Deneyiniz...");
                    goto tekrardtarih;
                }
                else
                {
                    string gun = tarih.Substring(0, 2);
                    if (Convert.ToInt16(gun) > 31 || Convert.ToInt16(gun) < 1)
                    {
                        Console.WriteLine("Hatalı Gün girdiniz. Tekrar Deneyiniz...");
                        goto tekrardtarih;

                    }
                    else
                    {
                        string ay = tarih.Substring(3, 2);
                        if (Convert.ToInt16(ay) > 12 || Convert.ToInt16(ay) < 1)
                        {
                            Console.WriteLine("Hatalı Ay Girdiniz. Tekrar Deneyiniz...");
                            goto tekrardtarih;
                        }
                        else
                        {
                            string yil = tarih.Substring(6, 4);
                            if (Convert.ToInt16(yil) > 3000 || Convert.ToInt16(yil)<1500)
                            {
                                Console.WriteLine("Hatalı Yıl Girdiniz. Tekrar Deneyiniz...");
                                goto tekrardtarih;
                            }
                            else
                            {
                                goto devamdtarih;
                            }
                        }
                    }
                }
            }
            devamdtarih:
            DateTime dtarih = DateTime.Parse(tarih);
            int sTarih = DateTime.Now.Year;
            int yas = sTarih - dtarih.Year;
            annegeri: 
            Console.WriteLine("Hastanın Anne Adını Giriniz: ");
            String anneAdi;
            String a_Adi = Console.ReadLine();
            if (a_Adi.Length <= 8)
            {
                anneAdi = a_Adi;
            }
            else
            {
                Console.WriteLine("Lütfen 9 Karakterden Kısa Metin Giriniz.");
                goto annegeri;
            }
            babageri:
            Console.WriteLine("Hastanın Baba Adını Giriniz: ");
            String babaAdi;
            String b_Adi = Console.ReadLine();
            if (b_Adi.Length <= 8)
            {
                babaAdi = b_Adi;
            }
            else
            {
                Console.WriteLine("Lütfen 9 Karakterden Kısa Metin Giriniz.");
                goto babageri;
            }
            Console.WriteLine("Hastanın Kan Grubunu Giriniz: ");
            String kanGrup = Console.ReadLine();
            tekrarboy:
            Console.WriteLine("Hastanın Boyunu Giriniz: ");
            String boyGir = Console.ReadLine();
            if (boyGir.IsNumeric())
            {
                goto devamboy;
            }
            else
            {
                Console.WriteLine("Hatalı Boy girdiniz. Tekrar Deneyiniz...");
                goto tekrarboy;
            }
            devamboy:
            Int32 boy = int.Parse(boyGir);
            if (boy >= 0 && boy <= 350)
            {
                goto devamboy1;
            }
            else
            {
                Console.WriteLine("Hatalı Boy Girdiniz. Tekrar Deneyiniz...");
                goto tekrarboy;
            }
            devamboy1:
            tekrarkilo:
            Console.WriteLine("Hastanın Kilosunu Giriniz: ");
            String kiloGir = Console.ReadLine();
            if (kiloGir.IsNumeric())
            {
                goto devamkilo;
            }
            else
            {
                Console.WriteLine("Hatalı Boy girdiniz. Tekrar Deneyiniz...");
                goto tekrarkilo;
            }
            devamkilo:
            Int32 kilo = int.Parse(kiloGir);
            if (boy >= 0 && boy <= 350)
            {
                goto devamkilo1;
            }
            else
            {
                Console.WriteLine("Hatalı Boy Girdiniz. Tekrar Deneyiniz...");
                goto tekrarkilo;
            }
            devamkilo1:
            tekrartel:
            Console.WriteLine("Hastanın Telefon Numarasını Giriniz:");
            String telGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtel;
            }
            else
            {
                Console.WriteLine("Hatalı Telefon Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrartel;
            }
            devamtel:
            Double tel = double.Parse(telGir);
            if (tcNo >= 0 && tcNo <= 99999999999)
            {
                goto devamtel1;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrartel;
            }
            devamtel1:
            string hasta = "hasta";


            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "INSERT INTO hOtomasyon (ad, soyAd, tcNo, cinsiyet,kanGrup,anneAdi,babaAdi,dTarih,yas,boy,kilo,tel,hasta) VALUES ('" + ad + "','" + soyAd + "','" + tcNo + "','"+cinsiyet+"','" + kanGrup + "','" + anneAdi + "','" + babaAdi + "','" + dtarih + "','" + yas + "','" + boy + "','" + kilo + "','" + tel + "','" + hasta + "')";
         
            baglanti.Open();
            int sonuc = komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
            if (sonuc > 0)
            {
                Console.WriteLine("\n Kayıt Başarıyla Eklendi");
            }
            else
            {
                Console.WriteLine("\n Kayıt Başarısız");
            }
        }
        //─────────────────────────────────────────────────────────────────────── Hasta Güncelleme ───────
        static void Guncelle()
        {
        tekrar:
            Console.WriteLine("Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:
        adgeri:
            Console.WriteLine("Hastanın Adını Giriniz: ");
            String ad;
            String ad_kontrol = Console.ReadLine();
            if (ad_kontrol.Length <= 10)
            {
                ad = ad_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 11 Karakterden Kısa Metin Giriniz");
                goto adgeri;
            }
        soyadgeri:
            Console.WriteLine("Hastanın Soyadını Giriniz");
            String soyAd;
            String soyad_kontrol = Console.ReadLine();
            if (soyad_kontrol.Length <= 9)
            {
                soyAd = soyad_kontrol;
            }
            else
            {
                Console.WriteLine("Lütfen 10 Karakterden Kısa Metin Giriniz");
                goto soyadgeri;
            }
            Console.WriteLine("Hastanın Cinsiyetini Giriniz Erkek[E] - Kadın[K]");
            string cinsiyet = Console.ReadLine();
            if ((cinsiyet == "E") || (cinsiyet == "e"))
            {
                cinsiyet = "Erkek";
            }
            else if ((cinsiyet == "K") || (cinsiyet == "k"))
            {
                cinsiyet = "Kadın";
            }
            else
            {
                Console.WriteLine("Lütfen Geçerli Cinsiyet Giriniz!");
            }
        tekrardtarih:
            Console.WriteLine("Hastanın Doğum Tarihini Giriniz: ");
            string tarih = Console.ReadLine();
            int uzunluk = tarih.Length;
            if (uzunluk > 10 || uzunluk < 8)
            {
                Console.WriteLine("Geçersiz Tarih Uzunluğu Girdiniz. Tekrar Deneyiniz...");
                goto tekrardtarih;
            }
            else
            {
                if (!(tarih.Substring(2, 1) == "." || tarih.Substring(5, 1) == "."))
                {
                    Console.WriteLine("Geçersiz Tarih Formatı Girdiniz. Tekrar Deneyiniz...");
                    goto tekrardtarih;
                }
                else
                {
                    string gun = tarih.Substring(0, 2);
                    if (Convert.ToInt16(gun) > 31 || Convert.ToInt16(gun) < 1)
                    {
                        Console.WriteLine("Hatalı Gün girdiniz. Tekrar Deneyiniz...");
                        goto tekrardtarih;

                    }
                    else
                    {
                        string ay = tarih.Substring(3, 2);
                        if (Convert.ToInt16(ay) > 12 || Convert.ToInt16(ay) < 1)
                        {
                            Console.WriteLine("Hatalı Ay Girdiniz. Tekrar Deneyiniz...");
                            goto tekrardtarih;
                        }
                        else
                        {
                            string yil = tarih.Substring(6, 4);
                            if (Convert.ToInt16(yil) > 3000 || Convert.ToInt16(yil) < 1500)
                            {
                                Console.WriteLine("Hatalı Yıl Girdiniz. Tekrar Deneyiniz...");
                                goto tekrardtarih;
                            }
                            else
                            {
                                goto devamdtarih;
                            }
                        }
                    }
                }
            }
        devamdtarih:
            DateTime dtarih = DateTime.Parse(tarih);
            int sTarih = DateTime.Now.Year;
            int yas = sTarih - dtarih.Year;
        annegeri:
            Console.WriteLine("Hastanın Anne Adını Giriniz: ");
            String anneAdi;
            String a_Adi = Console.ReadLine();
            if (a_Adi.Length <= 8)
            {
                anneAdi = a_Adi;
            }
            else
            {
                Console.WriteLine("Lütfen 9 Karakterden Kısa Metin Giriniz.");
                goto annegeri;
            }
        babageri:
            Console.WriteLine("Hastanın Baba Adını Giriniz: ");
            String babaAdi;
            String b_Adi = Console.ReadLine();
            if (b_Adi.Length <= 8)
            {
                babaAdi = b_Adi;
            }
            else
            {
                Console.WriteLine("Lütfen 9 Karakterden Kısa Metin Giriniz.");
                goto babageri;
            }
            Console.WriteLine("Hastanın Kan Grubunu Giriniz: ");
            String kanGrup = Console.ReadLine();
        tekrarboy:
            Console.WriteLine("Hastanın Boyunu Giriniz: ");
            String boyGir = Console.ReadLine();
            if (boyGir.IsNumeric())
            {
                goto devamboy;
            }
            else
            {
                Console.WriteLine("Hatalı Boy girdiniz. Tekrar Deneyiniz...");
                goto tekrarboy;
            }
        devamboy:
            Int32 boy = int.Parse(boyGir);
            if (boy >= 0 && boy <= 350)
            {
                goto devamboy1;
            }
            else
            {
                Console.WriteLine("Hatalı Boy Girdiniz. Tekrar Deneyiniz...");
                goto tekrarboy;
            }
        devamboy1:
        tekrarkilo:
            Console.WriteLine("Hastanın Kilosunu Giriniz: ");
            String kiloGir = Console.ReadLine();
            if (kiloGir.IsNumeric())
            {
                goto devamkilo;
            }
            else
            {
                Console.WriteLine("Hatalı Boy girdiniz. Tekrar Deneyiniz...");
                goto tekrarkilo;
            }
        devamkilo:
            Int32 kilo = int.Parse(kiloGir);
            if (boy >= 0 && boy <= 350)
            {
                goto devamkilo1;
            }
            else
            {
                Console.WriteLine("Hatalı Boy Girdiniz. Tekrar Deneyiniz...");
                goto tekrarkilo;
            }
        devamkilo1:
        tekrartel:
            Console.WriteLine("Hastanın Telefon Numarasını Giriniz:");
            String telGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtel;
            }
            else
            {
                Console.WriteLine("Hatalı Telefon Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrartel;
            }
        devamtel:
            Double tel = double.Parse(telGir);
            if (tcNo >= 0 && tcNo <= 99999999999)
            {
                goto devamtel1;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrartel;
            }
        devamtel1:
            

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "Update hOtomasyon Set ad=@ad, soyAd=@soyAd, dTarih=@dTarih, anneAdi=@anneAdi,babaAdi=@babaAdi,kanGrup=@kanGrup,boy=@boy,kilo=@kilo Where tcNo=@tcNo";
            
            komut.Parameters.AddWithValue("@ad", ad);
            komut.Parameters.AddWithValue("@soyAd",soyAd);
            komut.Parameters.AddWithValue("@dTarih",dtarih);
            komut.Parameters.AddWithValue("@anneAdi",anneAdi);
            komut.Parameters.AddWithValue("@babaAdi",babaAdi);
            komut.Parameters.AddWithValue("@kanGrup",kanGrup);
            komut.Parameters.AddWithValue("@boy",boy);
            komut.Parameters.AddWithValue("@kilo",kilo);
            komut.Parameters.AddWithValue("@tcNo", tcNo);
            
            baglanti.Open();
            komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
        }
        //─────────────────────────────────────────────────────────────────────── Hasta Silme ────────────
        static void Sil()
        {
        tekrar:
            Console.WriteLine("X TC Numaralı Hastayı Sil: ");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
        devam:

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "DELETE from hOtomasyon WHERE tcNo=" + tcNo + "";
            
            baglanti.Open();
            komut.ExecuteNonQuery();
            komut.Dispose();
            baglanti.Close();
        }
        //─────────────────────────────────────────────────────────────────────── Soyisme Göre Arama ─────
        static void Arama()
        {
            int sayac = 0;
            Console.WriteLine("Hasta Soyadını Giriniz: ");
            String soyAd = Console.ReadLine();
                        
            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE soyAd='" + soyAd + "'";
            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("TC NO\t\tYAŞ\tKAN GRUBU    CİNSİYET\tBOY\tKİLO\t  POLİKLİNİK\t\tDOKTOR\t\t  ANNE ADI\t  BABA ADI\tSOYAD\tAD\t\tTELEFON");
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[2] + "\t" + oku[7] + "\t" + oku[3] + " \t     " + oku[11] + "\t" + oku[8] + "\t" + oku[9] + "\t" + oku[10] + "     \t" + oku[12] + "   \t" + oku[4] + "\t  " + oku[5] + "\t\t" + oku[1] + "\t" + oku[0]+"\t"+oku[14]);
            }
            baglanti.Close();

        }
        //########################################## LİSTELEME İŞLEMİ ####################################
        //───────────────────────────────────────────────────────────────────── Poliklinik Listeleme ─────
        public static void pListele()
        {
            int sayac = 0;
            Console.WriteLine("Poliklinik Giriniz: ");
            string poliklinik = Console.ReadLine();

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE poliklinik='" + poliklinik + "'";
            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("TC NO\t\tYAŞ\tKAN GRUBU    CİNSİYET\tBOY\tKİLO\t  POLİKLİNİK\t\tDOKTOR\t\t  ANNE ADI\t  BABA ADI\tSOYAD\t\tAD");
            Console.WriteLine("────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[2] + "\t" + oku[7] + "\t" + oku[3] + " \t     " + oku[11] + "\t" + oku[8] + "\t" + oku[9] + "\t" + oku[10] + "     \t" + oku[12] + "   \t" + oku[4] + "\t  " + oku[5] + "\t\t" + oku[1] + "\t\t" + oku[0]);
            }
            baglanti.Close();
            Console.WriteLine("Toplam " + sayac + " Kişi Listelenmiştir");
        }
        //─────────────────────────────────────────────────────────────────────── Randevu Listeleme ──────
        public static void rListele()
        {
            int sayac = 0;
            String r_Kontrol = "R_Alindi";

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE r_Kontrol='" + r_Kontrol + "'";
            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("TC NO\t\tYAŞ\tKAN GRUBU    CİNSİYET\tBOY\tKİLO\t  POLİKLİNİK\t\tDOKTOR\t\t  ANNE ADI\t  BABA ADI\tSOYAD\t\tAD");
            Console.WriteLine("────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[2] + "\t" + oku[7] + "\t" + oku[3] + " \t     " + oku[11] + "\t" + oku[8] + "\t" + oku[9] + "\t" + oku[10] + "     \t" + oku[12] + "   \t" + oku[4] + "\t  " + oku[5] + "\t\t" + oku[1] + "\t\t" + oku[0]);
            }
            baglanti.Close();
            Console.WriteLine("Toplam " + sayac + " Kişi Listelenmiştir");
        }
        //─────────────────────────────────────────────────────────────────────── Cinsiyet Listeleme ─────
        public static void cListele()
        {
            int sayac = 0;
            atla:
            Console.WriteLine("Cinsiyet (Erkek[E] , Kadın[K]) Giriniz : ");
            string cinsiyet = Console.ReadLine();
            if ((cinsiyet == "E") || (cinsiyet == "e"))
            {
                cinsiyet = "Erkek";
            }
            else if ((cinsiyet == "K") || (cinsiyet == "k"))
            {
                cinsiyet = "Kadın";
            }
            else
            {
                Console.WriteLine("Lütfen Geçerli Cinsiyet Giriniz!");
                goto atla;
            }

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE cinsiyet='" + cinsiyet + "'";
            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("TC NO\t\tYAŞ\tKAN GRUBU    CİNSİYET\tBOY\tKİLO\t  POLİKLİNİK\t\tDOKTOR\t\t  ANNE ADI\t  BABA ADI\tSOYAD\t\tAD");
            Console.WriteLine("────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[2] + "\t" + oku[7] + "\t" + oku[3] + " \t     " + oku[11] + "\t" + oku[8] + "\t" + oku[9] + "\t" + oku[10] + "     \t" + oku[12] + "   \t" + oku[4] + "\t  " + oku[5] + "\t\t" + oku[1] + "\t\t" + oku[0]);
            }
            baglanti.Close();
            Console.WriteLine("Toplam " + sayac + " Kişi Listelenmiştir");
        }
        //─────────────────────────────────────────────────────────────────────── Doktora Göre Listeleme ─
        public static void dListele()
        {
            int sayac = 0;
            Console.WriteLine("Lütfen Doktor Adını Giriniz: ");
            string doktorAdi = Console.ReadLine();

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE doktorAdi='" + doktorAdi + "'";
            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("TC NO\t\tYAŞ\tKAN GRUBU    CİNSİYET\tBOY\tKİLO\t  POLİKLİNİK\t\tDOKTOR\t\t  ANNE ADI\t  BABA ADI\tSOYAD\t\tAD");
            Console.WriteLine("────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[2] + "\t" + oku[7] + "\t" + oku[3] + " \t     " + oku[11] + "\t" + oku[8] + "\t" + oku[9] + "\t" + oku[10] + "     \t" + oku[12] + "   \t" + oku[4] + "\t  " + oku[5] + "\t\t" + oku[1] + "\t\t" + oku[0]);
            }
            baglanti.Close();
            Console.WriteLine("Toplam " + sayac + " Kişi Listelenmiştir");
        }
        //─────────────────────────────────────────────────────────────────────── Hasta Sorgulama ────────
        public static void hSorgu()
        {
            int sayac = 0;
            tekrar:
            Console.WriteLine("Lütfen Sorgulamak İstediğiniz Hastanın TC Kimlik Numarasını Giriniz:");
            String tcGir = Console.ReadLine();
            if (tcGir.IsNumeric())
            {
                goto devamtc;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
            devamtc:
            Double tcNo = double.Parse(tcGir);
            if (tcNo >= 10000000000 && tcNo <= 99999999999)
            {
                goto devam;
            }
            else
            {
                Console.WriteLine("Hatalı TC Kimlik Numarası Girdiniz. Tekrar Deneyiniz...");
                goto tekrar;
            }
            devam:

            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE tcNo=@tcNo";
            komut.Parameters.AddWithValue("@tcNo", tcNo);


            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("KAYIT NO  ILETISIM\tYAŞ\tKAN GRUBU\tCİNSİYET\tBOY\tKİLO\tPOLİKLİNİK\t\tDOKTOR\t ANNE ADI\tBABA ADI\tSOYAD\tAD");
            Console.WriteLine("────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[13]+"\t "+oku[14] + "\t" + oku[7] + "\t" + oku[3] + " \t\t" + oku[11] + "\t\t" + oku[8] + "\t" + oku[9] + "\t" + oku[10] + "\t   " + oku[12] + "\t  " + oku[4] + "\t" + oku[5] + "\t\t" + oku[1] + "\t" + oku[0]);
            }
            baglanti.Close();
            Console.WriteLine("Toplam " + sayac + " Kişi Listelenmiştir");
        }
        //########################################## DOKTOR MENUSU #######################################
        //───────────────────────────────────────────────── X Doktordan Randevu Alan Hastaların Listesi ──
        static void dListe()
        {
            int sayac = -1;
            Console.WriteLine("Lütfen Doktor Adını Giriniz: ");
            string doktorAdi = Console.ReadLine();
            
            baglanti = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=hOtomasyon.mdb");
            komut = new OleDbCommand();
            komut.Connection = baglanti;
            komut.CommandText = "SELECT * FROM hOtomasyon WHERE doktorAdi='" + doktorAdi + "'";
            baglanti.Open();
            oku = komut.ExecuteReader();

            Console.WriteLine("SIRA NO\tRANDEVU SAATİ\tRANDEVU TARİHİ    \tPOLİKLİNİK\tŞİKAYETLER\tTAHLİLLER\tİLAÇLAR\t\tTEŞHİS\t\tSOYAD\tAD\t        EK");
            Console.WriteLine("──────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────────");
            while (oku.Read())
            {
                sayac++;
                Console.WriteLine(oku[13] + "\t" + oku[16] + "\t\t" + oku[15] + " \t" + oku[10] + "\t" + oku[19] + "\t" + oku[18] + "     \t" + oku[17] + "\t" + oku[20] + "\t" + oku[1] + "\t" + oku[0] + "\t" + oku[22] );
            }
            baglanti.Close();
            Console.WriteLine("Toplam " + sayac + " Kişi Listelenmiştir");
        }
        //########################################## HAKKINDA ############################################
        static void Hakkinda()
        {
            
                    
            Console.WriteLine();
            Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
            Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
            Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
            Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
            Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
            Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
            Console.WriteLine("\n \t\tBu Program 'Nesne Yönelimli Programlamaya Giriş' Dersi Proje Ödevi Olarak Hazırlanmıştır!");
            Console.WriteLine("\t\tBu Program Tamamen Deneysel Olup, Bir Çok Hata ile Karşılaşmanız Olası Bir Durumdur.");
            Console.WriteLine("\t\tBu Gibi Durumlarda Heyecana ve Umutsuzluğa Kapılmadan Klavyeden \"Alt+F4\" Tuşlarına veya");
            Console.WriteLine("\t\tSağ-Üst Köşedeki(Monitörün Kuzey-Doğu Koordinatındaki) \"X\" Butonuna Kibarca Basmanız Yeterli Olacaktır.");
            Console.WriteLine("\t\tBilgisayarınıza ve Programıma Zarar Vermemenizi Önemle Arz Ediyorum.");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t   İyi Günler :)");
            Console.WriteLine("\t\t\t\t\t\t\t\t\t\t\t\t\t\t\t Ömer Faruk ALMACI");
            Console.WriteLine("");
            Console.WriteLine("\t\t\t\t\t\t\t\tGeliştirici Bilgileri\n");
            Console.WriteLine("\t\t\t\t\t\t\tAdı Soyadı: \t Ömer Faruk ALMACI");
            Console.WriteLine("\t\t\t\t\t\t\tOkul:   \t Süleyman Demirel Üniversitesi");
            Console.WriteLine("\t\t\t\t\t\t\tFakülte: \t Teknoloji Fakültesi");
            Console.WriteLine("\t\t\t\t\t\t\tBölüm:   \t Mekatronik Mühendisliği (2.Öğretim)");
            Console.WriteLine("\t\t\t\t\t\t\tSınıf | Okul No: 2-A | 1522709001\n");
            Console.WriteLine("\t\t\t\t\t\t\tE-Posta: \t omeralmaci@gmail.com");
            Console.WriteLine("\t\t\t\t\t\t\tWeb:  \t \t www.omeralmaci.net");
            Console.WriteLine("\t\t\t\t\t\t\tgithub: \t @omeralmaci");
            Console.WriteLine("\t\t\t\t\t\t\tLinkedIN: \t /in/omeralmaci");
            Console.WriteLine("\t\t\t\t\t\t\tGplus:  \t +ÖmerFarukAlmacı");
            Console.WriteLine("\t\t\t\t\t\t\tTwitter: \t @omeralmaci");
            Console.WriteLine("\n\t\t\t\t\t\t\tFikren Katkıda Bulunanlar: ");
            Console.WriteLine("\t\t\t\t\t\t\t\t  Sinan Uğuz");
            Console.WriteLine("\t\t\t\t\t\t\t\t Mahsun Tursun");
            Console.WriteLine("\t\t\t\t\t\t\t\t  Onur Koltuk");

            Console.WriteLine("\nÇıkmak İçin Bir Tuşa Basınız!");
            Console.ReadKey();
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
            Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
            Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
            Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
            Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
            Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
        }
        //########################################## BAŞLANGIÇ EKRANI ####################################
        public static void Main(string[] args)
        {
            Console.Title = "SDÜ Hastane Otomasyonu - 1.1";
            Console.ResetColor();
            
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.BackgroundColor = ConsoleColor.White;

            ShowWindow(ThisConsole, BUYULTMEK);

        ilk:
            Console.WriteLine();
            Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
            Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
            Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
            Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
            Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
            Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
            
            int secim;

            do
            {
                secim = MenuListele();
                switch (secim)
                {
                    case 1: // Hasta Kayıt Oluşturma    ------------------
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
                        Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
                        Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
                        Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
                        Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
                        Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
                        Console.WriteLine("\n\t\t\t\t\t\t\t\t    Hasta Seçenekleri\n");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [1] Hasta Kayıt");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [2] Hasta Güncelle");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [3] Hasta Sil");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [4] Hasta Sorgula");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [5] Hasta Listele");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [6] Hasta Arama");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [G] Geri");
                        Console.Write("\t\tSeçiminiz: ");
                        int hSecenek1;
                        hastadon:
                        string hSecenek = Console.ReadLine();
                        if ((hSecenek =="1") || (hSecenek== "2") || (hSecenek=="3") || (hSecenek=="4") || (hSecenek=="5") || (hSecenek=="6")|| (hSecenek=="G") || (hSecenek=="g"))
                        {
                            if (hSecenek=="g" || hSecenek=="G")
                            {
                                hSecenek = "7";
                                hSecenek1 = int.Parse(hSecenek);
                            }
                            else
                            {
                                hSecenek1 = int.Parse(hSecenek);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                            Console.Write("Seçiminiz: ");
                            goto hastadon;
                        }
                        switch (hSecenek1)
                        {
                            case 1:
                                hKodon:
                                Console.WriteLine("Yeni Hasta Kaydı Oluşturmak Üzeresiniz..");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                String hSecim = Console.ReadLine();
                                if (hSecim == "e" || hSecim == "E")
                                {
                                    Kayit();
                                }
                                else if (hSecim == "h" || hSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hKodon;
                                }
                                break;
                            case 2:
                                hKgdon:
                                Console.WriteLine("Varolan Hasta Bilgilerini Güncellemek Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                hSecim = Console.ReadLine();
                                if (hSecim == "e" || hSecim == "E")
                                {
                                    Guncelle();
                                }
                                else if (hSecim == "h" || hSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hKgdon;
                                }

                                break;
                            case 3:
                                hKsdon:
                                Console.WriteLine("Varolan Hastayı Silmek Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                hSecim = Console.ReadLine();
                                if (hSecim == "e" || hSecim == "E")
                                {
                                    Sil();
                                }
                                else if (hSecim == "h" || hSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hKsdon;
                                }
                                break;
                            case 4:
                                hSdon:
                                Console.WriteLine("Hasta Bilgilerini Sorgulamak Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                hSecim = Console.ReadLine();
                                if (hSecim == "e" || hSecim == "E")
                                {
                                    hSorgu();
                                    break;
                                }
                                else if (hSecim == "h" || hSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hSdon;
                                }
                            case 5:
                                hLdon:
                                Console.WriteLine("Hasta Listelerini Almak Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                hSecim = Console.ReadLine();
                                if (hSecim == "e" || hSecim == "E")
                                {
                                    listedon:
                                    Console.WriteLine("\n");
                                    Console.WriteLine("\t[1] Poliklinik Listesi");
                                    Console.WriteLine("\t[2] Cinsiyet Listesi");
                                    Console.WriteLine("\t[3] X Doktorun Hastalarını Listele");
                                    Console.Write("Seçiminiz :");
                                    string sec = Console.ReadLine();
                                    int sec1;
                                    if ((sec == "1") || (sec == "2") || (sec == "3"))
                                    {
                                        sec1 = int.Parse(sec);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Hatalı Giriş Yaptınız, Tekrar Deneyiniz!");
                                        goto listedon;
                                    }
                                    switch (sec1)
                                    {
                                        case 1:
                                            pListele();
                                            break;
                                        case 2:
                                            cListele();
                                            break;
                                        case 3:
                                            dListele();
                                            break;
                                    }
                                }
                                else if (hSecim == "h" || hSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hLdon;
                                }
                                break;
                            case 6:
                                hAdon:
                                Console.WriteLine("X Soyadlı Hastayı/Hastaları Aramak Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                hSecim = Console.ReadLine();
                                if (hSecim == "e" || hSecim == "E")
                                {
                                    Arama();
                                }
                                else if (hSecim == "h" || hSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hAdon;
                                }

                                break;                                
                            case 7:
                                Console.Clear();
                                goto ilk;
                               }
                        
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
                        Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
                        Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
                        Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
                        Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
                        Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
                        Console.WriteLine("\n\t\t\t\t\t\t\t\t    Randevu İşlemleri\n");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [1] Randevu Kayıt");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [2] Randevu Güncelle");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [3] Randavu Sil");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [4] Randevu Listesi");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [G] Geri");
                        Console.Write("\t\tSeçiminiz: ");
                        int rSecenek1;
                        randevudon:
                        string rSecenek = Console.ReadLine();
                        if ((rSecenek == "1") || (rSecenek == "2") || (rSecenek == "3") || (rSecenek == "4") || (rSecenek == "G") || (rSecenek=="g"))
                        {
                            if (rSecenek == "g" || rSecenek == "G")
                            {
                                rSecenek = "4";
                                rSecenek1 = int.Parse(rSecenek);
                            }
                            else
                            {
                                rSecenek1 = int.Parse(rSecenek);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                            Console.Write("Seçiminiz: ");
                            goto randevudon;
                        }                        
                        switch (rSecenek1)
                        {
                            case 1:
                                rKodon:
                                Console.WriteLine("Hasta Randevu Kaydı Oluşturmak Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                string rSecim = Console.ReadLine();

                                if (rSecim == "e" || rSecim == "E")
                                {
                                    RandevuKayit();
                                    break;
                                }
                                else if (rSecim == "h" || rSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto rKodon;
                                }
                              
                            case 2:
                                rKgdon:
                                Console.WriteLine("Hastanın Varolan Randevu Bilgilerini Güncellemek Üzerisiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                rSecim = Console.ReadLine();
                                if (rSecim == "e" || rSecim == "E")
                                {
                                    RandevuGuncelle();
                                    break;
                                }
                                else if (rSecim == "h" || rSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto rKgdon;
                                }
                             
                            case 3:
                                rKsdon:
                                Console.WriteLine("Varolan Randevuyu Silmek Üzeresiniz");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                rSecim = Console.ReadLine();
                                if (rSecim == "e" || rSecim == "E")
                                {
                                    RandevuSil();
                                }
                                else if (rSecim == "h" || rSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto rKsdon;
                                }
                                break;
                            case 4:
                            rLgdon:
                                Console.WriteLine("Varolan Randevulu Hasta Bilgilerini Listelemek Üzerisiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                rSecim = Console.ReadLine();
                                if (rSecim == "e" || rSecim == "E")
                                {
                                    rListele();
                                    break;
                                }
                                else if (rSecim == "h" || rSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto rLgdon;
                                }

                        }

                        break;

                    case 3:
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
                        Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
                        Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
                        Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
                        Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
                        Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
                        Console.WriteLine("\n\t\t\t\t\t\t\t\t    Muayene İşlemleri\n");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [1] Muayene Kayıt");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [2] Muayene Güncelle");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [3] Muayene Sil");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [G] Geri");
                        Console.Write("\t\tSeçiminiz: ");
                        int mSecenek1;
                       muayenedon:
                        string mSecenek = Console.ReadLine();
                        if ((mSecenek == "1") || (mSecenek == "2") || (mSecenek == "3") || (mSecenek == "G") || (mSecenek =="g"))
                        {
                            if (mSecenek == "g" || mSecenek == "G")
                            {
                                mSecenek = "4";
                                mSecenek1 = int.Parse(mSecenek);
                            }
                            else
                            {
                                mSecenek1 = int.Parse(mSecenek);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                            Console.Write("Seçiminiz: ");
                            goto muayenedon;
                        }
                        switch (mSecenek1)
                        {
                            case 1:
                                hMkdon:
                                Console.WriteLine("Hasta Muayene Kaydı Oluşturmak Üzeresiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                string mSecim = Console.ReadLine();
                                if (mSecim == "e" || mSecim == "E")
                                {
                                    MuayeneKayit();
                                    break;
                                }
                                else if (mSecim == "h" || mSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto hMkdon;
                                }
                                
                            case 2:
                                mKgdon:
                                Console.WriteLine("Hastanın Varolan Randevu Bilgilerini Güncellemek Üzerisiniz.");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                string mGSecim = Console.ReadLine();
                                if (mGSecim == "e" || mGSecim == "E")
                                {
                                    RandevuGuncelle();
                                    break;
                                }
                                else if (mGSecim == "h" || mGSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto mKgdon;
                                }                                
                            case 3:
                                mKsdon:
                                Console.WriteLine("Varolan Randevuyu Silmek Üzeresiniz");
                                Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                                Console.Write("Seçiminiz: ");
                                string mKSecim = Console.ReadLine();
                                if (mKSecim == "e" || mKSecim == "E")
                                {
                                    RandevuSil();
                                }
                                else if (mKSecim == "h" || mKSecim == "H")
                                {
                                    Console.Clear();
                                    goto ilk;
                                }
                                else
                                {
                                    Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                                    goto mKsdon;
                                }
                                break;
                            case 4:
                                Console.Clear();
                                goto ilk;
                                
                        }                        
                        break;
                    case 4:
                        int dokSec;
                        Console.Clear();
                        Console.WriteLine();
                        Console.WriteLine("\t\t\t\t\t\t\t┌─────────────────────────────────────────┐");
                        Console.WriteLine("\t\t\t\t\t\t\t│      Süleyman Demirel Üniversitesi      │");
                        Console.WriteLine("\t\t\t\t\t\t\t│        Hastane Otomasyon Sistemi        │");
                        Console.WriteLine("\t\t\t\t\t\t\t│     developer by  Ömer Faruk Almacı     │");
                        Console.WriteLine("\t\t\t\t\t\t\t│          Ver: 1.1   build 0287          │");
                        Console.WriteLine("\t\t\t\t\t\t\t└─────────────────────────────────────────┘");
                        Console.WriteLine("\n\t\t\t\t\t\t\t\t    Doktor İşlemleri\n");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [1] Randevu Listeniz");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [2] Randevu Sil");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [3] ");
                        Console.WriteLine("\t\t\t\t\t\t\t\t    [G] Geri");
                        dokbas:
                        Console.WriteLine("Seçiminiz: ");
                        string dSeccim = Console.ReadLine();
                        
                        if ((dSeccim == "1") || (dSeccim=="2") || (dSeccim=="3") || (dSeccim=="g") || (dSeccim=="G"))
                        {
                            if (dSeccim == "g" || dSeccim=="G")
                            {
                                dSeccim = "4";
                                dokSec = int.Parse(dSeccim);
                            }
                            else
                            {
                                dokSec = int.Parse(dSeccim);      
                            }
                         }
                        else
                        {
                            Console.WriteLine("Yanlış Seçim Yaptınız, Tekrar Deneyiniz");
                            goto dokbas;
                        }
                        switch (dokSec)
                        {
                            case 1:
                                
                                dListe();
                                break;
                            case 2:
                                RandevuSil();
                                break;
                            case 3:
                                break;
                            case 4:
                                Console.Clear();
                                goto ilk;
                                
                        }

                        break;
                    case 5:
                        cikDon:
                        Console.WriteLine("Programdan Çıkmak Üzeresiniz.");
                        Console.WriteLine("Bu Yaptığını İşlemden Emin iseniz [E], Geri Dönmek İstiyorsanız [H] Yazınız.");
                        Console.Write("Seçiminiz: ");
                        string cSecim = Console.ReadLine();
                        if (cSecim == "e" || cSecim == "E")
                        {
                            break;
                        }
                        else if (cSecim == "h" || cSecim == "H")
                        {
                            Console.Clear();
                            goto ilk;


                        }
                        else
                        {
                            Console.WriteLine("Yanlış Seçim Yaptınız Lütfen Tekrar Deneyiniz.");
                            goto cikDon;
                        }

                    case 6:
                        Console.Clear();
                        Console.Beep();
                        Hakkinda();
                        break;                                          
                }
            } while (secim != 5);
        }
    }
}