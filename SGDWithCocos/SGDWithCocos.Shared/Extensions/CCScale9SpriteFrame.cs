/*
 * CocosSharp is licensed under the MIT license terms, in a similar spirit to cocos2d for Objective-C and Cocos-2D with C++.
 * 
 * https://github.com/mono/CocosSharp
 *  
 */

namespace CocosSharp
{
    public class CCScale9SpriteFrame : CCScale9Sprite
    {
        #region Constructors

        public CCScale9SpriteFrame(CCSpriteFrame spriteFrame, CCRect capInsets) : base(spriteFrame, capInsets)
        {
        }

        public CCScale9SpriteFrame(CCSpriteFrame spriteFrame) : base(spriteFrame)
        {
        }

        public CCScale9SpriteFrame(string spriteFrameName, CCRect capInsets) : base()
        {
            // Can't call base(string,...) because we're using the string parameter for file names
            base.InitWithSpriteFrameName(spriteFrameName, capInsets);
        }

        public CCScale9SpriteFrame(string alias)
        {
            // Can't call base(string,...) because we're using the string parameter for file names
            base.InitWithSpriteFrameName(alias);
        }

        #endregion Constructors
    }
}
