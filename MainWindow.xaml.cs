using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace NativeGameHub
{
    public partial class MainWindow : Window
    {
        private List<GameInfo> _allGames = new();
        private List<GameInfo> _filteredGames = new();
        private GameInfo? _selectedGame;
        private DispatcherTimer _clockTimer;
        private CancellationTokenSource? _boostCts;
        private string _activeFilter = "all";

        public MainWindow()
        {
            InitializeComponent();
            InitClock();
            DetectHardwareAsync();
            LoadGames();
        }

        private void DetectHardwareAsync()
        {
            Task.Run(() =>
            {
                var specs = HardwareDetector.Detect();
                Dispatcher.Invoke(() =>
                {
                    HudCpuText.Text = $"⚡ {specs.Cpu}";
                    HudGpuText.Text = $"🎮 {specs.Gpu}";
                    HudRamText.Text = $"💾 {specs.Ram}";
                });
            });
        }

        private void InitClock()
        {
            _clockTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _clockTimer.Tick += (s, e) => UpdateClock();
            _clockTimer.Start();
            UpdateClock();
        }

        private void UpdateClock()
        {
            var now = DateTime.Now;
            ClockTimeText.Text = now.ToString("HH:mm");
            string[] days = { "CHỦ NHẬT", "THỨ HAI", "THỨ BA", "THỨ TƯ", "THỨ NĂM", "THỨ SÁU", "THỨ BẢY" };
            ClockDateText.Text = $"{days[(int)now.DayOfWeek]}, {now.Day}/{now.Month}";
        }

        private void LoadGames()
        {
            _allGames = new List<GameInfo>
            {
                new GameInfo
                {
                    Id = "hitman3",
                    Name = "HITMAN World of Assassination",
                    Developer = "IO Interactive",
                    Genre = "Hành Động / Ám Sát Lén Lút",
                    Release = "2021",
                    Description = "Nhập vai sát thủ số 1 thế giới - Đặc vụ 47 chu du khắp địa cầu từ tháp chọc trời Dubai, dinh thự Anh quốc đến dãy núi tuyết Carpathian để triệt phá tổ chức ngầm bí ẩn Providence.",
                    ExecutablePath = @"E:\HITMAN.3.v3.190_LinkNeverDie.Com\Retail\HITMAN3.exe",
                    Arguments = "-skip_launcher",
                    WorkingDirectory = @"E:\HITMAN.3.v3.190_LinkNeverDie.Com\Retail",
                    Folder = @"E:\HITMAN.3.v3.190_LinkNeverDie.Com",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1659040/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1659040/library_hero.jpg",
                    ThemeColor = "#E63946",
                    Tags = new List<string> { "🎯 Tối ưu 60 FPS", "⚡ FSR 2.0", "🏆 Full Trilogy", "🇻🇳 Việt Hóa" }
                },
                new GameInfo
                {
                    Id = "re4remake",
                    Name = "Resident Evil 4 Remake",
                    Developer = "Capcom",
                    Genre = "Sinh Tồn / Kinh Dị / Hành Động",
                    Release = "2023",
                    Description = "Cùng đặc vụ kì cựu Leon S. Kennedy thâm nhập một ngôi làng hẻo lánh u ám ở Tây Ban Nha để giải cứu con gái tổng thống Ashley trước giáo phái ký sinh trùng Los Iluminados cuồng loạn.",
                    ExecutablePath = @"E:\Resident.Evil.4.b22154679_LinkNeverDie.Com\re4.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\Resident.Evil.4.b22154679_LinkNeverDie.Com",
                    Folder = @"E:\Resident.Evil.4.b22154679_LinkNeverDie.Com",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2050650/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2050650/library_hero.jpg",
                    ThemeColor = "#D90429",
                    Tags = new List<string> { "🇻🇳 Việt Hóa 100%", "🔥 Leon Kennedy", "⚡ RE Engine", "⭐ Siêu Phẩm" }
                },
                new GameInfo
                {
                    Id = "re8village",
                    Name = "Resident Evil Village",
                    Developer = "Capcom",
                    Genre = "Kinh Dị / Bắn Súng / Sinh Tồn",
                    Release = "2021",
                    Description = "Theo chân người cha Ethan Winters dấn thân vào ngôi làng tuyết phủ quái dị vùng Đông Âu, đối đầu với nữ chúa ma cà rồng Lady Dimitrescu và các đại chúa quỷ để tìm lại đứa con gái bé bỏng.",
                    ExecutablePath = @"E:\Resident.Evil.Village_LinkNeverDie.Com\re8.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\Resident.Evil.Village_LinkNeverDie.Com",
                    Folder = @"E:\Resident.Evil.Village_LinkNeverDie.Com",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1196590/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1196590/library_hero.jpg",
                    ThemeColor = "#2B8A3E",
                    Tags = new List<string> { "🇻🇳 Việt Hóa 100%", "🌹 Shadows of Rose", "🏰 Dimitrescu", "⭐ Full DLC" }
                },
                new GameInfo
                {
                    Id = "thantrung",
                    Name = "The Death - Thần Trùng",
                    Developer = "DUT Studio (Dũng CT)",
                    Genre = "Kinh Dị Tâm Linh Việt Nam",
                    Release = "2022",
                    Description = "Tác phẩm kinh dị tâm linh mang đậm dấu ấn phố phường Hà Nội của Trực Tiếp Game (Dũng CT). Bạn vào vai một chàng thanh niên mắc kẹt trong căn nhà trọ ma ám đầy những bí mật rùng rợn năm 2021.",
                    ExecutablePath = @"E:\The.Death.Than.Trung_LinkNeverDie.Com\The Death.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\The.Death.Than.Trung_LinkNeverDie.Com",
                    Folder = @"E:\The.Death.Than.Trung_LinkNeverDie.Com",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1726400/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1726400/library_hero.jpg",
                    ThemeColor = "#F03E3E",
                    Tags = new List<string> { "🇻🇳 Game Việt Nam", "🎙️ Dũng CT / DUT", "🏮 Phố Cổ Hà Nội" }
                },
                new GameInfo
                {
                    Id = "gtav",
                    Name = "Grand Theft Auto V",
                    Developer = "Rockstar Games",
                    Genre = "Thế Giới Mở / Bắn Súng / Đua Xe",
                    Release = "2015",
                    Description = "Bom tấn thế giới mở số 1 mọi thời đại. Tự do tung hoành trên đường phố Los Santos cùng bộ ba giang hồ Michael, Franklin và gã điên Trevor trong những phi vụ thế kỷ kịch tính.",
                    ExecutablePath = @"E:\Grand Theft Auto V Legacy\PlayGTAV.exe",
                    Arguments = "-nobattleye",
                    WorkingDirectory = @"E:\Grand Theft Auto V Legacy",
                    Folder = @"E:\Grand Theft Auto V Legacy",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/271590/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/271590/library_hero.jpg",
                    ThemeColor = "#E67700",
                    Tags = new List<string> { "🌴 Los Santos", "🇻🇳 Việt Hóa", "⚡ Mượt 60 FPS", "⭐ Huyền Thoại" }
                },
                new GameInfo
                {
                    Id = "wukong",
                    Name = "Black Myth: Wukong",
                    Developer = "Game Science",
                    Genre = "Hành Động / Nhập Vai / Thần Thoại",
                    Release = "2024",
                    Description = "Hóa thân thành Thiên Mệnh Nhân, dấn thân vào hành trình Tây Du đầy hiểm nguy và kỳ vĩ để tìm lại những mảnh lục căn thất lạc của Tề Thiên Đại Thánh Tôn Ngộ Không.",
                    ExecutablePath = @"E:\Black Myth Wukong\b1.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\Black Myth Wukong",
                    Folder = @"E:\Black Myth Wukong",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2358720/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/2358720/library_hero.jpg",
                    ThemeColor = "#E5A93C",
                    Tags = new List<string> { "🐒 Tề Thiên Đại Thánh", "🔥 Siêu Phẩm 2024", "⚡ Unreal Engine 5", "⭐ AAA Đỉnh Cao" }
                },
                new GameInfo
                {
                    Id = "cyberpunk2077",
                    Name = "Cyberpunk 2077: Phantom Liberty",
                    Developer = "CD PROJEKT RED",
                    Genre = "Thế Giới Mở / Bắn Súng / RPG",
                    Release = "2023",
                    Description = "Dấn thân vào thành phố tương lai rực rỡ ánh đèn neon Night City trong vai lính đánh thuê V cùng huyền thoại nhạc rock Johnny Silverhand (Keanu Reeves).",
                    ExecutablePath = @"E:\Cyberpunk 2077\bin\x64\Cyberpunk2077.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\Cyberpunk 2077",
                    Folder = @"E:\Cyberpunk 2077",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1091500/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1091500/library_hero.jpg",
                    ThemeColor = "#FCEE0A",
                    Tags = new List<string> { "🦾 Night City", "🔥 Keanu Reeves", "⚡ Siêu Phẩm Ray Tracing", "🚀 Cyberpunk" }
                },
                new GameInfo
                {
                    Id = "rdr2",
                    Name = "Red Dead Redemption 2",
                    Developer = "Rockstar Games",
                    Genre = "Thế Giới Mở / Viễn Tây / Hành Động",
                    Release = "2018",
                    Description = "Kiệt tác miền Tây hoang dã số 1 lịch sử làng game. Theo chân tay súng Arthur Morgan và băng đảng Van der Linde trong những năm tháng suy tàn của thời đại cao bồi tự do.",
                    ExecutablePath = @"E:\Red Dead Redemption 2\RDR2.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\Red Dead Redemption 2",
                    Folder = @"E:\Red Dead Redemption 2",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1174180/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1174180/library_hero.jpg",
                    ThemeColor = "#C1121F",
                    Tags = new List<string> { "🤠 Cao Bồi Viễn Tây", "🏆 Game Của Thập Kỷ", "🐎 Arthur Morgan", "⭐ Điểm Tuyệt Đối 10/10" }
                },
                new GameInfo
                {
                    Id = "eldenring",
                    Name = "Elden Ring",
                    Developer = "FromSoftware",
                    Genre = "Thế Giới Mở / Soulslike / Hành Động",
                    Release = "2022",
                    Description = "Vươn mình trở thành Chúa Tể Elden trong thế giới kỳ ảo Lands Between huyền bí được chắp bút bởi đạo diễn Hidetaka Miyazaki và tác giả George R.R. Martin.",
                    ExecutablePath = @"E:\ELDEN RING\Game\eldenring.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\ELDEN RING",
                    Folder = @"E:\ELDEN RING",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1245620/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1245620/library_hero.jpg",
                    ThemeColor = "#C69214",
                    Tags = new List<string> { "💍 Game Of The Year", "⚔️ FromSoftware", "🔥 Thế Giới Mở", "💀 Thử Thách Cực Hạn" }
                },
                new GameInfo
                {
                    Id = "godofwar",
                    Name = "God of War",
                    Developer = "Santa Monica Studio",
                    Genre = "Hành Động / Chặt Chém / Thần Thoại",
                    Release = "2022",
                    Description = "Chiến thần Kratos cùng con trai Atreus bước vào cuộc hành trình sinh tử qua cửu giới Bắc Âu đầy tuyết lạnh, đối đầu với những quái thú và các vị thần hùng mạnh.",
                    ExecutablePath = @"E:\God of War\GoW.exe",
                    Arguments = "",
                    WorkingDirectory = @"E:\God of War",
                    Folder = @"E:\God of War",
                    CoverUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1593500/library_600x900_2x.jpg",
                    HeroUrl = "https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/1593500/library_hero.jpg",
                    ThemeColor = "#3A86C8",
                    Tags = new List<string> { "🪓 Chiến Thần Kratos", "❄️ Thần Thoại Bắc Âu", "🎮 PlayStation Hit", "🏆 Đỉnh Cao Hành Động" }
                }
            };

            ApplyFilter("all");
        }

        private void ApplyFilter(string filter)
        {
            _activeFilter = filter;

            if (filter == "action")
            {
                _filteredGames = _allGames.Where(g => g.Genre.Contains("Hành Động") || g.Genre.Contains("Bắn Súng") || g.Genre.Contains("Thế Giới Mở") || g.Genre.Contains("Nhập Vai") || g.Genre.Contains("Chặt Chém")).ToList();
            }
            else if (filter == "horror")
            {
                _filteredGames = _allGames.Where(g => g.Genre.Contains("Kinh Dị") || g.Genre.Contains("Sinh Tồn")).ToList();
            }
            else if (filter == "installed")
            {
                _filteredGames = _allGames.Where(g => g.IsInstalled).ToList();
            }
            else
            {
                _filteredGames = new List<GameInfo>(_allGames);
            }

            TabAll.Content = $"Tất Cả ({_allGames.Count})";
            TabInstalled.Content = $"Đã Cài Đặt ({_allGames.Count(g => g.IsInstalled)})";

            RenderCards();

            if (_filteredGames.Count > 0)
            {
                SelectGame(_filteredGames[0]);
            }
        }

        private void FilterTab_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is string tag)
            {
                TabAll.Background = Brushes.Transparent;
                TabAll.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E"));

                TabAction.Background = Brushes.Transparent;
                TabAction.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E"));

                TabHorror.Background = Brushes.Transparent;
                TabHorror.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E"));

                TabInstalled.Background = Brushes.Transparent;
                TabInstalled.Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#8B949E"));

                btn.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#21262D"));
                btn.Foreground = Brushes.White;

                ApplyFilter(tag);
            }
        }

        private void RenderCards()
        {
            CardsContainer.Children.Clear();

            foreach (var game in _filteredGames)
            {
                var scaleTransform = new ScaleTransform(1.0, 1.0);
                var translateTransform = new TranslateTransform(0, 0);
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleTransform);
                transformGroup.Children.Add(translateTransform);

                var cardBorder = new Border
                {
                    Width = 148,
                    Height = 216,
                    CornerRadius = new CornerRadius(16),
                    Margin = new Thickness(0, 0, 18, 0),
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#141820")),
                    BorderBrush = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255)),
                    BorderThickness = new Thickness(1.5),
                    Cursor = Cursors.Hand,
                    Tag = game,
                    RenderTransform = transformGroup,
                    RenderTransformOrigin = new Point(0.5, 0.5)
                };

                // Tight clipping mask so image and gradients never bleed out of rounded corners
                var maskBorder = new Border
                {
                    Width = 148,
                    Height = 216,
                    CornerRadius = new CornerRadius(15),
                    Background = Brushes.Black
                };
                cardBorder.OpacityMask = new VisualBrush(maskBorder);

                var grid = new Grid();

                // Cover Image
                try
                {
                    var img = new Image
                    {
                        Source = new BitmapImage(new Uri(game.CoverUrl)),
                        Stretch = Stretch.UniformToFill
                    };
                    grid.Children.Add(img);
                }
                catch { }

                // Top Badge
                var badgeBorder = new Border
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(0, 8, 8, 0),
                    CornerRadius = new CornerRadius(8),
                    Background = new SolidColorBrush(Color.FromArgb(215, 0, 0, 0)),
                    Padding = new Thickness(7, 3, 7, 3)
                };
                var badgeText = new TextBlock
                {
                    Text = game.StatusBadgeText,
                    FontSize = 9.5,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString(game.StatusBadgeColor))
                };
                badgeBorder.Child = badgeText;
                grid.Children.Add(badgeBorder);

                // Bottom Gradient Title Overlay
                var titleOverlay = new Border
                {
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Background = new LinearGradientBrush(
                        Color.FromArgb(245, 9, 11, 14),
                        Color.FromArgb(0, 9, 11, 14),
                        90.0),
                    Padding = new Thickness(10, 24, 10, 10)
                };
                var titleBlock = new TextBlock
                {
                    Text = game.Name,
                    FontSize = 11.5,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    TextTrimming = TextTrimming.CharacterEllipsis
                };
                titleOverlay.Child = titleBlock;
                grid.Children.Add(titleOverlay);

                cardBorder.Child = grid;

                // Smooth Hover Lift Animation (kéo chuột qua lại nhô lên siêu mượt)
                cardBorder.MouseEnter += (s, e) =>
                {
                    if (s is Border b && b.Tag is GameInfo g)
                    {
                        var animY = new DoubleAnimation(-18.0, TimeSpan.FromMilliseconds(160))
                        {
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };
                        var animScale = new DoubleAnimation(1.06, TimeSpan.FromMilliseconds(160))
                        {
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };

                        translateTransform.BeginAnimation(TranslateTransform.YProperty, animY);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animScale);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animScale);

                        // Seamlessly update spotlight on hover
                        SelectGame(g);
                    }
                };

                cardBorder.MouseLeave += (s, e) =>
                {
                    if (s is Border b && b.Tag is GameInfo g)
                    {
                        bool isSelected = (_selectedGame?.Id == g.Id);
                        double targetY = isSelected ? -8.0 : 0.0;
                        double targetScale = isSelected ? 1.03 : 1.0;

                        var animY = new DoubleAnimation(targetY, TimeSpan.FromMilliseconds(200))
                        {
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };
                        var animScale = new DoubleAnimation(targetScale, TimeSpan.FromMilliseconds(200))
                        {
                            EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
                        };

                        translateTransform.BeginAnimation(TranslateTransform.YProperty, animY);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleXProperty, animScale);
                        scaleTransform.BeginAnimation(ScaleTransform.ScaleYProperty, animScale);
                    }
                };

                cardBorder.MouseLeftButtonDown += (s, e) =>
                {
                    if (s is Border b && b.Tag is GameInfo g)
                    {
                        SelectGame(g);
                    }
                };

                CardsContainer.Children.Add(cardBorder);
            }
        }

        private void SelectGame(GameInfo game)
        {
            _selectedGame = game;

            // Update Spotlight info
            DevText.Text = game.Developer;
            GenreText.Text = game.Genre;
            YearText.Text = game.Release;
            TitleText.Text = game.Name;
            DescText.Text = game.Description;

            // Update Dynamic Ambient Glow
            try
            {
                var themeColor = (Color)ColorConverter.ConvertFromString(game.ThemeColor);
                AmbientGlowStop.Color = themeColor;
            }
            catch { }

            // Update Backdrop
            try
            {
                BackdropImage.Source = new BitmapImage(new Uri(!string.IsNullOrEmpty(game.HeroUrl) ? game.HeroUrl : game.CoverUrl));
            }
            catch { }

            // Update Tags
            TagsPanel.Children.Clear();
            foreach (var tag in game.Tags)
            {
                var pill = new Border
                {
                    Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#181E27")),
                    BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2E3846")),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(12, 5, 12, 5),
                    Margin = new Thickness(0, 0, 8, 6)
                };
                pill.Child = new TextBlock
                {
                    Text = tag,
                    Foreground = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F6FC")),
                    FontSize = 11.5,
                    FontWeight = FontWeights.SemiBold
                };
                TagsPanel.Children.Add(pill);
            }

            // Update Active Card visual with tight rounded borders
            foreach (var child in CardsContainer.Children)
            {
                if (child is Border b && b.Tag is GameInfo g)
                {
                    if (g.Id == game.Id)
                    {
                        var col = (Color)ColorConverter.ConvertFromString(game.ThemeColor);
                        b.BorderBrush = new SolidColorBrush(col);
                        b.BorderThickness = new Thickness(2.5);
                        b.Effect = new DropShadowEffect
                        {
                            Color = col,
                            BlurRadius = 26,
                            ShadowDepth = 0,
                            Opacity = 0.65
                        };
                        b.BringIntoView();
                    }
                    else
                    {
                        b.BorderBrush = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255));
                        b.BorderThickness = new Thickness(1.5);
                        b.Effect = null;
                    }
                }
            }

            // Update Play button color and text depending on installation status
            if (PlayBtn.Template.FindName("BtnBorder", PlayBtn) is Border border)
            {
                border.Background = game.IsInstalled
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString(game.ThemeColor))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#21262D"));
            }
            if (PlayBtn.Template.FindName("PlayBtnText", PlayBtn) is TextBlock btnText)
            {
                btnText.Text = game.IsInstalled ? "CHƠI NGAY" : "CHƯA CÀI ĐẶT";
                btnText.Foreground = game.IsInstalled
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#090B0E"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#C9D1D9"));
            }
            if (PlayBtn.Template.FindName("PlayBtnIcon", PlayBtn) is TextBlock btnIcon)
            {
                btnIcon.Text = game.IsInstalled ? "▶" : "○";
                btnIcon.Foreground = game.IsInstalled
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#090B0E"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString(game.StatusBadgeColor));
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGame == null) return;

            if (!_selectedGame.IsInstalled)
            {
                NotInstalledGameNameText.Text = _selectedGame.Name;
                NotInstalledDescText.Text = $"Tựa game '{_selectedGame.Name}' hiện chưa có trên máy tính của bạn (đang trưng bày dạng Showcase Demo trong bộ sưu tập).\n\nKhi nào bạn tải hoặc cài đặt game vào ổ cứng (E:\\...), Aurora Game Hub sẽ tự động nhận diện và sẵn sàng khởi động!";
                NotInstalledOverlay.Visibility = Visibility.Visible;
                return;
            }

            // Show Popup confirmation dialog
            ChoiceGameNameText.Text = _selectedGame.Name;
            LaunchChoiceOverlay.Visibility = Visibility.Visible;
        }

        private void ChoiceDirectLaunch_Click(object sender, RoutedEventArgs e)
        {
            LaunchChoiceOverlay.Visibility = Visibility.Collapsed;
            if (_selectedGame != null)
            {
                DirectLaunch(_selectedGame);
            }
        }

        private async void ChoiceBoostLaunch_Click(object sender, RoutedEventArgs e)
        {
            LaunchChoiceOverlay.Visibility = Visibility.Collapsed;
            if (_selectedGame != null)
            {
                await StartBoostSequenceAsync(_selectedGame);
            }
        }

        private void CancelLaunchChoice_Click(object sender, RoutedEventArgs e)
        {
            LaunchChoiceOverlay.Visibility = Visibility.Collapsed;
        }

        private async Task StartBoostSequenceAsync(GameInfo game)
        {
            _boostCts?.Cancel();
            _boostCts = new CancellationTokenSource();

            BoostGameNameText.Text = game.Name;
            BoostProgressBar.Value = 5;
            BoostStatusText.Text = "Chuẩn bị kích hoạt Gaming Turbo Boost...";
            BoostDetailsText.Text = "Đang tối ưu hệ thống và mở rộng VRAM cho Intel UHD Graphics";
            BoostOverlay.Visibility = Visibility.Visible;

            try
            {
                await GameBooster.RunBoostAsync(progress =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        BoostProgressBar.Value = progress.Percent;
                        BoostStatusText.Text = progress.Status;
                        BoostDetailsText.Text = progress.Details;
                    });
                }, _boostCts.Token);
            }
            catch (OperationCanceledException)
            {
                // Skipped by user
            }
            finally
            {
                BoostOverlay.Visibility = Visibility.Collapsed;
                DirectLaunch(game);
            }
        }

        private void SkipBoost_Click(object sender, RoutedEventArgs e)
        {
            _boostCts?.Cancel();
        }

        private void DirectLaunch(GameInfo game)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = game.ExecutablePath,
                    Arguments = game.Arguments,
                    WorkingDirectory = !string.IsNullOrEmpty(game.WorkingDirectory)
                        ? game.WorkingDirectory
                        : Path.GetDirectoryName(game.ExecutablePath),
                    UseShellExecute = true // Native Windows ShellExecuteEx
                };

                var proc = Process.Start(psi);
                if (proc != null)
                {
                    GameBooster.BoostGameProcess(proc);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Không thể khởi động game:\n{ex.Message}", "Lỗi Khởi Động", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedGame == null) return;

            if (Directory.Exists(_selectedGame.Folder))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = $"\"{_selectedGame.Folder}\"",
                    UseShellExecute = true
                });
            }
            else
            {
                MessageBox.Show($"Thư mục game không tồn tại:\n{_selectedGame.Folder}", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CardsScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            CardsScrollViewer.ScrollToHorizontalOffset(CardsScrollViewer.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (_filteredGames.Count == 0) return;

            int currentIndex = _selectedGame != null ? _filteredGames.FindIndex(g => g.Id == _selectedGame.Id) : 0;

            if (e.Key == Key.Right)
            {
                int nextIndex = (currentIndex + 1) % _filteredGames.Count;
                SelectGame(_filteredGames[nextIndex]);
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                int prevIndex = (currentIndex - 1 + _filteredGames.Count) % _filteredGames.Count;
                SelectGame(_filteredGames[prevIndex]);
                e.Handled = true;
            }
            else if (e.Key == Key.Enter)
            {
                Play_Click(this, new RoutedEventArgs());
                e.Handled = true;
            }
            else if (e.Key == Key.F11)
            {
                ToggleMaximize();
                e.Handled = true;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                ToggleMaximize();
                return;
            }
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            ToggleMaximize();
        }

        private void ToggleMaximize()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
                if (MaximizeBtn != null) MaximizeBtn.Content = "▢";
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                if (MaximizeBtn != null) MaximizeBtn.Content = "❐";
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseNotInstalledOverlay_Click(object sender, RoutedEventArgs e)
        {
            NotInstalledOverlay.Visibility = Visibility.Collapsed;
        }
    }
}
