using MobileCongreso;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using CustomRederer.Android;
[assembly: ExportRenderer(typeof(CustomEntry), typeof(CustomEntryRenderer))]
namespace CustomRederer.Android
{
    public class CustomEntryRenderer: EntryRenderer
    {
        public CustomEntryRenderer(Context context): base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if(Control != null)
            {
                GradientStrokeDrawable gradient = new GradientStrokeDrawable();
                gradient.SetColor(global::Android.Graphics.Color.Transparent);
                Control.SetBackground(gradient);
                this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
                Control.SetHintTextColor(global::Android.Graphics.Color.Transparent);
            }
        }
    }
}