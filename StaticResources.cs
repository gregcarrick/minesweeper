using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Minesweeper
{
    public static class StaticResources
    {
        public static Brush BgBrush = (Brush)Application.Current.Resources["BgBrush"];
        public static Brush HighlightBrush = (Brush)Application.Current.Resources["HighlightBrush"];
        public static Brush ShadowBrush = (Brush)Application.Current.Resources["ShadowBrush"];
        public static Brush MouseOverBrush = (Brush)Application.Current.Resources["MouseOverBrush"];

        public static BitmapImage DetonatedMineImage = (BitmapImage)Application.Current.Resources["DetonatedMineImage"];
        public static BitmapImage FlagImage = (BitmapImage)Application.Current.Resources["FlagImage"];
        public static BitmapImage MineImage = (BitmapImage)Application.Current.Resources["MineImage"];
    }
}
