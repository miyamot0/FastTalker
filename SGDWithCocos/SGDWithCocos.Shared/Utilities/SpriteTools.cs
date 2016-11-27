using CocosSharp;
using SGDWithCocos.Tags;

namespace SGDWithCocos.Utilities
{
    public static class SpriteTools
    {
        /// <summary>
        /// Static call, extract label
        /// </summary>
        /// <param name="sprite"></param>
        /// <returns></returns>
        public static string SpriteHasLabel(CCSprite sprite)
        {
            CCLabel contentTag = sprite.GetChildByTag(SpriteTypes.ContentTag) as CCLabel;

            if (contentTag != null)
            {
                return contentTag.Text;
            }
            else
            {
                return "";
            }
        }
    }
}
