// Copyright (c) 2007 Adrian Godong, Ben Maurer
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

// Adapted for dotnetblogengine by Filip Stanek ( http://www.bloodforge.com )

namespace Recaptcha
{
    using System;

    /// <summary>
    /// Recaptcha Log Item
    /// </summary>
    [Serializable]
    public class RecaptchaLogItem
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref = "RecaptchaLogItem" /> class.
        /// </summary>
        public RecaptchaLogItem()
        {
            this.Response = String.Empty;
            this.Necessary = true;
            this.Enabled = true;
            this.CommentId = Guid.Empty;
            this.Challenge = String.Empty;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the challenge.
        /// </summary>
        /// <value>The challenge.</value>
        public string Challenge { get; set; }

        /// <summary>
        ///     Gets or sets the comment id.
        /// </summary>
        /// <value>The comment id.</value>
        public Guid CommentId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref = "RecaptchaLogItem" /> is enabled.
        /// </summary>
        /// <value><c>true</c> if enabled; otherwise, <c>false</c>.</value>
        public bool Enabled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref = "RecaptchaLogItem" /> is necessary.
        /// </summary>
        /// <value><c>true</c> if necessary; otherwise, <c>false</c>.</value>
        public bool Necessary { get; set; }

        /// <summary>
        ///     Gets or sets the number of attempts.
        /// </summary>
        /// <value>The number of attempts.</value>
        public ushort NumberOfAttempts { get; set; }

        /// <summary>
        ///     Gets or sets the response.
        /// </summary>
        /// <value>The response.</value>
        public string Response { get; set; }

        /// <summary>
        ///     Gets or sets the time to comment.
        /// </summary>
        /// <remarks>
        ///     in seconds - this is the time from the initial page load until a captcha was successfully solved
        /// </remarks>
        public double TimeToComment { get; set; }

        /// <summary>
        ///     Gets or sets the time to solve capcha.
        /// </summary>
        /// <remarks>
        ///     in seconds - this is the time from the last time the captcha was refreshed until it was successfully solved.
        /// </remarks>
        public double TimeToSolveCapcha { get; set; }

        #endregion
    }
}