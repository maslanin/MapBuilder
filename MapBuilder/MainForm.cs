using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MapBuilder
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Picturebox_resize();
        }

        readonly Settings fs = new Settings();
        public int zoom = 0;                // ну как бы во скока раз картинку увеличим
        public static string way = "";      // путь к папке с файлами
        private bool grid = false;          // рисовать ли сетку
        public List<Node> waypoints;        // вейпоинты, загружается 1 раз
        Bitmap minimapa;                    // миникарта, загружается 1 раз
        Bitmap prohodimostj;                // проходимость, загружается 1 раз
        Bitmap mp;                          // будем работать с копией
        IniFile MyIni;                      // ини файл вейпоинтов, загружается 1 раз
        List<Teleports> tlp;                // список телепортов
        int countpoints = 0;                // количество вейпоинтов
        int[,] level;                       // карта проходимости
        public int idx = -1;                // индекс для вставки вейпоинта
        bool cng = false;                   // флаг что мы изменяем вейпоинт с ID = idx
        public static string settings = ""; // настройки
        public bool backstart = false;      // флаг поиска пути от последней точки к первой
        private void OpenFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(var fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = way;
                DialogResult result = fbd.ShowDialog();
                if (way == fbd.SelectedPath)
                    return; // нет смысла считывать одно и то же 2й раз
                way = fbd.SelectedPath;
                if (result == DialogResult.Cancel || string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    return;
                if(!System.IO.File.Exists(way + "\\minimapa.bmp"))
                {
                    MessageBox.Show("Missing file: minimapa.bmp", "Error!");
                    return;
                }
                if (!System.IO.File.Exists(way + "\\prohodimostj.bmp"))
                {
                    MessageBox.Show("Missing file: prohodimostj.bmp", "Error!");
                    return;
                }
                if (!System.IO.File.Exists(way + "\\waipoint.ini"))
                {
                    MessageBox.Show("Missing file:: waipoint.ini, a new one will be created", "Warning!");
                    string set = "";
                    set += "vini2vzgljada=1" + Environment.NewLine;
                    set += "vini1monstr=" + Environment.NewLine;
                    set += "vini2monstr=" + Environment.NewLine;
                    set += "vinives41=" + Environment.NewLine;
                    set += "vinives42=" + Environment.NewLine;
                    set += "vinives43=" + Environment.NewLine;
                    set += "vinives44=" + Environment.NewLine;
                    set += "vinives45=" + Environment.NewLine;
                    set += "vinives46=" + Environment.NewLine;
                    set += "vinives47=" + Environment.NewLine;
                    set += "vinives48=" + Environment.NewLine;
                    set += "vinives49=";
                    fs.Clear();
                    System.IO.File.WriteAllText(@way + "\\waipoint.ini", "[waip]\r\n\r\n[nastrojki]\r\n" + set);
                }
                // далее пытаемся загрузить карты и вейпоинты
                this.Text = new System.IO.DirectoryInfo(way).Name;
                // обнуляем переменные перед открытием НОВОЙ карты
                minimapa = null;
                mp = null;
                prohodimostj = null;
                MyIni = null;
                waypoints = null;
                tlp = null;
                level = null;
                idx = -1;
                cng = false;
                settings = "";
                //grid = false;
                //backstart = false;
                LoadMap(way);
                saveAsProjectToolStripMenuItem.Enabled = true;
                settingsToolStripMenuItem.Enabled = true;
            }
            return;
        }

        private void LoadMap(string ways)
        {
            Picturebox_resize();
            if (minimapa == null) minimapa = new Bitmap(@ways + "\\minimapa.bmp");              // загружаем картинку карты
            if (prohodimostj == null) prohodimostj = new Bitmap(@ways + "\\prohodimostj.bmp");  // загружаем картинку проходимости
            if (MyIni == null) MyIni  = new IniFile(@ways + "\\waipoint.ini");                  // загружаем файл с вейпоинтами
            mp = (Bitmap)minimapa.Clone();                                                      // создаем копию миникарты, чтобы не портить рисованием основную миникарту
            string fa;
            // вычитаем настройки из ини
            string[] sett = new string[11];
            sett[0] = MyIni.Read("vini1monstr", "nastrojki");
            sett[1] = MyIni.Read("vini2monstr", "nastrojki");
            sett[2] = MyIni.Read("vinives41", "nastrojki");
            sett[3] = MyIni.Read("vinives42", "nastrojki");
            sett[4] = MyIni.Read("vinives43", "nastrojki");
            sett[5] = MyIni.Read("vinives44", "nastrojki");
            sett[6] = MyIni.Read("vinives45", "nastrojki");
            sett[7] = MyIni.Read("vinives46", "nastrojki");
            sett[8] = MyIni.Read("vinives47", "nastrojki");
            sett[9] = MyIni.Read("vinives48", "nastrojki");
            sett[10] = MyIni.Read("vinives49", "nastrojki");
            fs.AddSettings(sett);
            // дальше мы найдем максимальный номер шага от 0 до ...
            if (waypoints == null)
            {
                for (int i = 0; ; i++)
                {
                    countpoints = i;
                    try
                    {
                        fa = MyIni.Read("waipNomerSleduwegoHoda" + i, "waip");
                        if (Int32.Parse(fa) == 0)
                            break;
                    }
                    catch
                    {
                        // на случай пустого ini
                        break;
                    }
                }
            }
            else
            {
                countpoints = waypoints.Count - 1;
            }
            int widthdiff = pictureBox1.Width / mp.Width; // разница между шириной картинки и шириной пикчер бокса
            int heightdiff = pictureBox1.Height / mp.Height; // разница между высотой картинки и высотой пикчер бокса
            zoom = (heightdiff <= widthdiff) ? heightdiff : widthdiff; // выберем меньшее число
            level = new int[mp.Width, mp.Height]; // карта проходимости
            for (int j = 0; j < mp.Height; j++)
            {
                for (int i = 0; i < mp.Width; i++)
                {
                    Color c = mp.GetPixel(i, j);
                    Color p = prohodimostj.GetPixel(i, j);        // возьмем только цвет из минимапы для замены белого в проходимости
                    if (Color.Black.ToArgb() != p.ToArgb())
                    {
                        level[i, j] = 0;                    // белый цвет - есть проход
                    }
                    else
                    {
                        level[i, j] = 1;                    // нет прохода
                    }
                    if (Color.FromArgb(255, 0, 255).ToArgb() == c.ToArgb()) // уберем для красоты цвет 0xFF00FF
                    {
                        mp.SetPixel(i, j, Color.Black);
                    }
                }
            }

            // расставим точки от вейпоинтов
            if (waypoints == null)
            {
                waypoints = new List<Node>(); // создадим пустой список вейпоинтов
                for (int i = 0; i <= countpoints; i++)
                {
		    if (countpoints == 0)
                        break; // если нет ни одного вейпоинта
                    try
                    {
                        int obr = 0;
                        string kuda = "";
                        string wag = "";
                        string imag = "";
                        int zapret = 0;
                        // далее читаем всё из ини.
                        try
                        {
                            fa = MyIni.Read("waipVariantObrabo4ika4kp" + i, "waip");    // читаем тип вейпоинта
                            obr = int.Parse(fa);
                        }
                        catch(Exception e)
                        {
                            MessageBox.Show("Unable to read VariantObrabo4ika4kp: " + Environment.NewLine + e.Message);
                            return;
                        }
                        if (obr == 0)
                        {
                            try 
                            {
                                fa = MyIni.Read("waipPunktNazna4enija" + i, "waip");
                                kuda = fa;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Unable to read PunktNazna4enija: " + Environment.NewLine + e.Message);
                                return;
                            }
                        }
                        else if (obr == 2 || obr == 3)
                        {
                            try
                            {
                                fa = MyIni.Read("waipTo4kaPribitija" + i, "waip");
                                kuda = fa;
                            }
                            catch (Exception e)
                            {
                                MessageBox.Show("Unable to read  To4kaPribitija:" + Environment.NewLine + e.Message);
                                return;
                            }
                        }
                        try
                        {
                            fa = MyIni.Read("waipStoronaWaga" + i, "waip");    // читаем тип вейпоинта
                            wag = fa;
                        }
                        catch (Exception)
                        {
                            wag = "";
                        }
                        try
                        {
                            fa = MyIni.Read("waipImjaTeleporta" + i, "waip");    // читаем тип вейпоинта
                            imag = fa;
                        }
                        catch (Exception)
                        {
                            imag = "";
                        }
                        try
                        {
                            fa = MyIni.Read("waipZapretPoiska" + i, "waip");    // читаем тип вейпоинта
                            zapret = int.Parse(fa);
                        }
                        catch (Exception)
                        {
                            zapret = 0;
                        }
                        // конец чтения из ини

                        // получим координаты
                        String[] pnt23 = kuda.Split(new string[] { "m" }, StringSplitOptions.None); // разберем координату на X и Y
                        int X = int.Parse(pnt23[0]) - 1; // -1 потому что на фото координаты с 0:0 начинаются
                        int Y = int.Parse(pnt23[1]) - 1;
                        if (X < 0) X = 0;
                        if (Y < 0) Y = 0;
                        if (X >= mp.Width) X = mp.Width - 1;
                        if (Y >= mp.Height) Y = mp.Height - 1;

                        // добавляем класс в лист
                        waypoints.Add(new Node
                        {
                            Obrabot4ik = obr,
                            PunktNazna4enija = kuda,
                            StoronaWaga = wag,
                            ImjaTeleporta = imag,
                            ZapretPoiska = zapret,
                            X = X,
                            Y = Y
                        }
                            );
                    }
                    catch
                    {
                        // на случай пустого ini
                        break;
                    }
                }
            }
            
            // тут астар до зума картинки
            for (int i = 0; i <= countpoints; i ++)
            {
                try
                {
                    List<Point> doroga;
                    if (i == countpoints && backstart == true) // последняя интерация цикла
                    {
                        doroga = AStar.FindPath(level, new Point(waypoints[i].X, waypoints[i].Y), new Point(waypoints[0].X, waypoints[0].Y)); // между последним и первым вейпоинтом
                    }
                    else
                    {
                        doroga = AStar.FindPath(level, new Point(waypoints[i].X, waypoints[i].Y), new Point(waypoints[i + 1].X, waypoints[i + 1].Y)); // между одним и предыдущим
                    }
                    if (doroga != null)
                    {
                        foreach (Point pnt33 in doroga)
                        {
                            mp.SetPixel(pnt33.X, pnt33.Y, Color.DarkBlue);
                        }
                    }
                }
                catch
                {
                     break;
                }
            }

            // добавим телепорты, если есть
			for (int i = 0; i <= countpoints; i ++)
			{
				try
				{
                    // teleports
                    if (waypoints[i].Obrabot4ik != 0)
                    {
                        if (tlp == null)
                        {
                            tlp = new List<Teleports>();
                        }
                        tlp.Add(new Teleports
                        {
                            X1 = waypoints[i - 1].X,
                            Y1 = waypoints[i - 1].Y,
                            X2 = waypoints[i].X,
                            Y2 = waypoints[i].Y
                        }
                            );
                    }
				}
				catch
				{
					break;
				}
			}

            // нарисуем точки вейпоинтов
            for (int i = 0; i <= countpoints; i ++)
            {
                try
                {
                    mp.SetPixel(waypoints[i].X, waypoints[i].Y, Color.DarkRed);
                }
                catch
                {
                    break;
                }
            }

            Bitmap nmapa = new Bitmap(mp.Width * zoom, mp.Height * zoom, mp.PixelFormat); // создадим новую пустую картинку
            Graphics g = Graphics.FromImage(nmapa);
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor; // отключим сглаживание
            g.DrawImage(mp, new Rectangle(Point.Empty, nmapa.Size)); // зальем пустую картинку зазумленным изображением

            if (tlp != null)
            {
                for (int i = 0; i < tlp.Count; i++)
                {
                    if (tlp[i].X1 != tlp[i].X2 && tlp[i].Y1 != tlp[i].Y2)
                    {
                        Pen p = new Pen(Color.Green);
                        g.DrawLine(p, tlp[i].X1 * zoom + zoom / 2, tlp[i].Y1 * zoom + zoom / 2, tlp[i].X2 * zoom + zoom / 2, tlp[i].Y2 * zoom + zoom / 2);
                    }
                }
            }

            // нарисуем сетку если зум 8 или больше
            if (zoom >= 8 && grid)
            {
                for (int i = 0; i < mp.Width; i++)
                {
                    Pen p = new Pen(Color.White);
                    g.DrawLine(p, i * zoom, 0, i * zoom, mp.Height * zoom);
                }
                for (int i = 0; i < mp.Height; i++)
                {
                    Pen p = new Pen(Color.White);
                    g.DrawLine(p, 0, i * zoom, mp.Width * zoom, i * zoom);
                }
            }

            pictureBox1.Image = nmapa; // выведем на пикчер бокс
            pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize; // изменим пикчер бокс до размера зазумленной картинки
            return;
        }

        private void PictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (zoom == 0)
                return;
            label1.Text = (e.X / zoom + 1) + "m" + (e.Y / zoom + 1);
            return;
        }

        private void Picturebox_resize() // надо бы переделать, тормозит при быстром изменении размера формы
        {
            pictureBox1.Width = this.Width - 48;
            pictureBox1.Height = this.Height - 76;
            return;
        }
        private void GridStatusOffToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (grid == true)
            {
                grid = false;
                gridStatusOffToolStripMenuItem.Text = "Grid Status: Off";
            }
            else
            {
                grid = true;
                gridStatusOffToolStripMenuItem.Text = "Grid Status: On";
            }
            MainForm_Resize(sender, e);
            return;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (zoom == 0)
            {
                Picturebox_resize();
            }
            else
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    pictureBox1.Image = null;
                }
                else
                {
                    pictureBox1.Image = null;
                    LoadMap(way);
                }
            }
        }

        private void SaveAsProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string name = new System.IO.DirectoryInfo(way).Name;
            saveFileDialog1.Filter = "PNG (*.png)|*.png|All Files (*.*)|*.*";
            saveFileDialog1.FileName = name + ".png";
            if (saveFileDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            string str = saveFileDialog1.FileName;

            // картинку возьмем уже готовую, с обработанным астаром. но мы ее в 10 раз увеличим, и нарисуем недостающие зеленые линии телепортов
            Bitmap nmapa = new Bitmap(mp.Width * 10, mp.Height * 10, mp.PixelFormat); // создадим новую пустую картинку
            Graphics g = Graphics.FromImage(nmapa);
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor; // отключим сглаживание
            g.DrawImage(mp, new Rectangle(Point.Empty, nmapa.Size)); // зальем пустую картинку зазумленным изображением

            if (tlp != null)
            {
                for (int i = 0; i < tlp.Count - 1; i++)
                {
                    if (tlp[i].X1 != tlp[i].X2 && tlp[i].Y1 != tlp[i].Y2)
                    {
                        Pen p = new Pen(Color.Green);
                        g.DrawLine(p, tlp[i].X1 * 10 + 5, tlp[i].Y1 * 10 + 5, tlp[i].X2 * 10 + 5, tlp[i].Y2 * 10 + 5);
                    }
                }
            }
            nmapa.Save(@str);
        }
        private void PictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (zoom == 0 || way == "")
                return;
            int X = e.X / zoom;
            int Y = e.Y / zoom;
            if (prohodimostj.GetPixel(X, Y).ToArgb() == Color.Black.ToArgb())
                return; // туда нельзя идти
            if (waypoints == null)
            {
                // это для создания нового маршрута
                waypoints = new List<Node>();
            }
            int oldX = 0;
            int oldY = 0;
            if (waypoints.Count > 0)
            {
                if (idx != -1)
                {
                    oldX = waypoints[idx].X;    // предыдущие координаты
                    oldY = waypoints[idx].Y;    // 
                }
                else
                {
                    oldX = waypoints[waypoints.Count - 1].X;    // предыдущие координаты
                    oldY = waypoints[waypoints.Count - 1].Y;    // 
                }
            }
            if (e.Button == MouseButtons.Right)
            {
                // по идее тут надо сделать меню вставки телепорта, лесенки или обычного вейпоинта + ссылку отмены
                ContextMenu m = new ContextMenu();
                if (waypoints.Count == 0 || AStar.FindPath(level, new Point(oldX, oldY), new Point(X, Y)) != null)
                {
                    // только если есть сюда дорога ИЛИ новый вейпоинт вообще
                    if (idx != -1)
                    {
                        MenuItem m_wpn = new MenuItem("Add waypoint (ID: " + idx + ")") // только если есть дорога от старой точки
                        {
                            Tag = X.ToString() + " " + Y.ToString()
                        };
                        m_wpn.Click += new EventHandler(Click_Addwpn);
                        m.MenuItems.Add(m_wpn);
                        MenuItem m_hidewpn = new MenuItem("Add Hide waypoint (ID: " + idx + ")")
                        {
                            Tag = X.ToString() + " " + Y.ToString()
                        };
                        m_hidewpn.Click += new EventHandler(Click_Addhidewpn);
                        m.MenuItems.Add(m_hidewpn);
                    }
                    else
                    {
                        MenuItem m_wpn = new MenuItem("Add waypoint") // только если есть дорога от старой точки
                        {
                            Tag = X.ToString() + " " + Y.ToString()
                        };
                        m_wpn.Click += new EventHandler(Click_Addwpn);
                        m.MenuItems.Add(m_wpn);
                        MenuItem m_hidewpn = new MenuItem("Add Hide waypoint")
                        {
                            Tag = X.ToString() + " " + Y.ToString()
                        };
                        m_hidewpn.Click += new EventHandler(Click_Addhidewpn);
                        m.MenuItems.Add(m_hidewpn);
                    }
                }
                if (0 <= waypoints.Count)
                {
                    // так как это точка прибытия, точкой отсправления считается предыдущий вейпоинт.
                    // так что телепорт или лесенка не могут быть первым вейпоинтом
                    MenuItem m_stair = new MenuItem("Add Stair")
                    {
                        Tag = X.ToString() + " " + Y.ToString()
                    };
                    m_stair.Click += new EventHandler(Click_Addstair);
                    m.MenuItems.Add(m_stair);
                    MenuItem m_teleport = new MenuItem("Add Teleport")
                    {
                        Tag = X.ToString() + " " + Y.ToString()
                    };
                    m_teleport.Click += new EventHandler(Click_Addteleport);
                    m.MenuItems.Add(m_teleport);
                }
                for (int i = 0; i < waypoints.Count; i++)
                {
                    if (waypoints[i].X == X && waypoints[i].Y == Y)
                    {
                        MenuItem m_change = new MenuItem("Change this (ID: " + i + ")")
                        {
                            Tag = i
                        };
                        m_change.Click += new EventHandler(Click_Change);
                        m.MenuItems.Add(m_change);
                        MenuItem m_delete = new MenuItem("Delete this (ID: " + i + ")")
                        {
                            Tag = i
                        };
                        m_delete.Click += new EventHandler(Click_Delete);
                        m.MenuItems.Add(m_delete);
                        MenuItem m_addafter = new MenuItem("Add after this (ID: " + i + ")")
                        {
                            Tag = i
                        };
                        m_addafter.Click += new EventHandler(Click_Addafter);
                        m.MenuItems.Add(m_addafter);
                    }
                }
                if (0 <= waypoints.Count)
                {
                    MenuItem m_clear = new MenuItem("Clear all");
                    m_clear.Click += new EventHandler(Clear_Click);
                    m.MenuItems.Add(m_clear);
                }
                if (idx != -1)
                {
                    // кнопка отмены, удаляет ID из вставки вейпоинта
                    MenuItem m_cancel = new MenuItem("Cancel");
                    m_cancel.Click += new EventHandler(Cancel_Click);
                    m.MenuItems.Add(m_cancel);
                }
                
                m.Show(pictureBox1, e.Location);
                return;
            }
            else if (e.Button == MouseButtons.Left)
            {
                // проверим предыдущий вейпоинт
                if  (oldX == X && oldY == Y)
                {
                    // вдруг двойной клик прошел
                    return;
                }
                if (waypoints.Count == 0 || AStar.FindPath(level, new Point(oldX, oldY), new Point(X, Y)) != null)
                {
                    // дорога туда есть
                    waypoints.Add(new Node
                    {
                        Obrabot4ik = 0,
                        PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                        StoronaWaga = "",
                        ImjaTeleporta = "",
                        ZapretPoiska = 0,
                        X = X,
                        Y = Y
                    });
                    Remap(waypoints, way);
                    return;
                }
            }
            return;
        }

        private void Click_Change(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            idx = int.Parse(b.Tag.ToString());
            if(waypoints[idx].Obrabot4ik != 0)
            {
                MessageBox.Show("You cannot change the point of a stair or teleport.");
                return;
            }
            cng = true;
        }

        private void Click_Addafter(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            idx = int.Parse(b.Tag.ToString());
        }

        private void Click_Delete(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            waypoints.RemoveAt(int.Parse(b.Tag.ToString()));
            Remap(waypoints, way);
        }

        private void Click_Addteleport(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            String[] param = b.Tag.ToString().Split(new string[] { " " }, StringSplitOptions.None);
            FormTeleport f3 = new FormTeleport()
            {
                Owner = this
            };
            int X = int.Parse(param[0]);
            int Y = int.Parse(param[1]);
            int oldX;
            int oldY;
            if (idx != -1)
            {
                oldX = waypoints[idx].X;
                oldY = waypoints[idx].Y;
            }
            else
            {
                oldX = waypoints[waypoints.Count - 1].X;
                oldY = waypoints[waypoints.Count - 1].Y;
            }
            f3.AddCoords(oldX, oldY, X, Y);
            f3.ShowDialog();
        }

        private void Click_Addstair(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            String[] param = b.Tag.ToString().Split(new string[] { " " }, StringSplitOptions.None);
            FormStair f2 = new FormStair()
            {
                Owner = this
            };
            int X = int.Parse(param[0]);
            int Y = int.Parse(param[1]);
            int oldX;
            int oldY;
            if (idx != -1)
            {
                oldX = waypoints[idx].X;
                oldY = waypoints[idx].Y;
            }
            else
            {
                oldX = waypoints[waypoints.Count - 1].X;
                oldY = waypoints[waypoints.Count - 1].Y;
            }
            f2.AddCoords(oldX, oldY, X, Y);
            f2.ShowDialog();
        }

        private void Click_Addhidewpn(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            String[] param = b.Tag.ToString().Split(new string[] { " " }, StringSplitOptions.None);
            int X = int.Parse(param[0]);
            int Y = int.Parse(param[1]);
            if (idx != -1)
            {
                // вставим по индексу, но хз как оно сдвинет остальное
                if (cng == true)
                {
                    waypoints[idx] = new Node
                    {
                        Obrabot4ik = 0,
                        PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                        StoronaWaga = "",
                        ImjaTeleporta = "",
                        ZapretPoiska = 1,
                        X = X,
                        Y = Y
                    };
                    cng = false;
                }
                else
                {
                    waypoints.Insert(idx + 1, new Node
                    {
                        Obrabot4ik = 0,
                        PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                        StoronaWaga = "",
                        ImjaTeleporta = "",
                        ZapretPoiska = 1,
                        X = X,
                        Y = Y
                    });
                }
                // обнулим переменную индекса
                idx = -1;
            }
            else
            {
                waypoints.Add(new Node
                {
                    Obrabot4ik = 0,
                    PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                    StoronaWaga = "",
                    ImjaTeleporta = "",
                    ZapretPoiska = 1,
                    X = X,
                    Y = Y
                });
            }
            Remap(waypoints, way);
        }

        private void Click_Addwpn(object sender, EventArgs e)
        {
            MenuItem b = sender as MenuItem;
            String[] param = b.Tag.ToString().Split(new string[] { " " }, StringSplitOptions.None);
            int X = int.Parse(param[0]);
            int Y = int.Parse(param[1]);
            if (idx != -1)
            {
                // вставим по индексу, но хз как оно сдвинет остальное
                if (cng == true)
                {
                    waypoints[idx] = new Node
                    {
                        Obrabot4ik = 0,
                        PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                        StoronaWaga = "",
                        ImjaTeleporta = "",
                        ZapretPoiska = 0,
                        X = X,
                        Y = Y
                    };
                    cng = false;
                }
                else
                {
                    waypoints.Insert(idx + 1, new Node
                    {
                        Obrabot4ik = 0,
                        PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                        StoronaWaga = "",
                        ImjaTeleporta = "",
                        ZapretPoiska = 0,
                        X = X,
                        Y = Y
                    });
                }
                // обнулим переменную индекса
                idx = -1;
            }
            else
            {
                waypoints.Add(new Node
                {
                    Obrabot4ik = 0,
                    PunktNazna4enija = (X + 1).ToString() + "m" + (Y + 1).ToString(),
                    StoronaWaga = "",
                    ImjaTeleporta = "",
                    ZapretPoiska = 0,
                    X = X,
                    Y = Y
                });
            }
            Remap(waypoints, way);
        }

        private void Clear_Click(object sender, EventArgs e)
        {
            waypoints = null;
            waypoints = new List<Node>();
            settings = "";
            tlp = null;
            tlp = new List<Teleports>();
            idx = -1;
            cng = false;
            Remap(waypoints, way);
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            idx = -1;
            cng = false;
        }

        public void Remap(List<Node> wpn, string ways)
        {
            waypoints = wpn;
            tlp = null;
            tlp = new List<Teleports>();
            pictureBox1.Image = null;
            LoadMap(ways);
            return;
        }

        private void SaveProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (waypoints == null)
                return;
            if (settings == "")
            {
                MessageBox.Show("First fill in the map settings!");
                return;
            }
            string txt = "[waip]" + Environment.NewLine;
            for (int i = 0; i <= countpoints; i ++)
            {
                txt += "; " + (i + 1).ToString() + " Point" + Environment.NewLine;
                txt += "waipVariantObrabo4ika4kp" + i.ToString() + "=" + waypoints[i].Obrabot4ik.ToString() + Environment.NewLine; // 0-лесенка
                if (i == countpoints)
                {
                    txt += "waipNomerSleduwegoHoda" + i.ToString() + "=0" + Environment.NewLine;
                }
                else
                {
                    txt += "waipNomerSleduwegoHoda" + i.ToString() + "=" + (i + 1).ToString() + Environment.NewLine;
                }
                if (waypoints[i].Obrabot4ik == 0)
                {
                    txt += "waipPunktNazna4enija" + i.ToString() + "=" + waypoints[i].PunktNazna4enija + Environment.NewLine;
                }
                else
                {
                    txt += "waipTo4kaOtpravlenija" + i.ToString() + "=" + waypoints[i - 1].PunktNazna4enija + Environment.NewLine;
                    txt += "waipTo4kaPribitija" + i.ToString() + "=" + waypoints[i].PunktNazna4enija + Environment.NewLine;
                    if (waypoints[i].Obrabot4ik == 3)
                    {
                        txt += "waipImjaTeleporta" + i.ToString() + "=" + waypoints[i].ImjaTeleporta;
                    }
                    txt += "waipStoronaWaga" + i.ToString() + "=" + waypoints[i].StoronaWaga + Environment.NewLine; 
                }
                if (waypoints[i].ZapretPoiska == 1)
                {
                    txt += "waipZapretPoiska" + i.ToString() + "=1";
                }
                txt += "waipZonirovanie" + i.ToString() + "=0" + Environment.NewLine;
            }
            if (txt != "")
            {
                txt += "[nastrojki]" + Environment.NewLine + settings;
                System.IO.File.WriteAllText(way + @"\waipoint.ini", txt);
                MessageBox.Show("Waypoints saved successfully!");
                return;
            }
            else
            {
                MessageBox.Show("Failed to save waypoints!");
            }
        }

        private void QuitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fs.ShowDialog();
        }

        private void BackwayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (backstart == true)
            {
                backstart = false;
                backwayToolStripMenuItem.Text = "Way to start: Off";
            }
            else
            {
                backstart = true;
                backwayToolStripMenuItem.Text = "Way to start: On";
            }
            Remap(waypoints, way);
        }
    }
}
