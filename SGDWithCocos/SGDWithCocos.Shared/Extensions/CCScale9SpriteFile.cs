﻿/*
 * CocosSharp is licensed under the MIT license terms, in a similar spirit to cocos2d for Objective-C and Cocos-2D with C++.
 * 
 * https://github.com/mono/CocosSharp
 *  
 */

namespace CocosSharp
{
    public class CCScale9SpriteFile : CCScale9Sprite
    {
        #region Constructors

        public CCScale9SpriteFile(string file, CCRect rect, CCRect capInsets) : base(file, rect, capInsets)
        {
        }

        public CCScale9SpriteFile(string file, CCRect rect) : base(file, rect)
        {
        }

        public CCScale9SpriteFile(CCRect capInsets, string file) : base(file, CCRect.Zero, capInsets)
        {
        }

        public CCScale9SpriteFile(string file) : base(file)
        {
        }

        #endregion Constructors
    }
}
