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
    /// <summary>
    /// The recaptcha response.
    /// </summary>
    public class RecaptchaResponse
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes static members of the <see cref="RecaptchaResponse"/> class. 
        /// </summary>
        static RecaptchaResponse()
        {
            Valid = new RecaptchaResponse(true, string.Empty);
            RecaptchaNotReachable = new RecaptchaResponse(false, "recaptcha-not-reachable");
            InvalidSolution = new RecaptchaResponse(false, "incorrect-captcha-sol");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecaptchaResponse"/> class.
        /// </summary>
        /// <param name="valid">
        /// if set to <c>true</c> [is valid].
        /// </param>
        /// <param name="errorCode">
        /// The error code.
        /// </param>
        internal RecaptchaResponse(bool valid, string errorCode)
        {
            this.IsValid = valid;
            this.ErrorCode = errorCode;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the invalid solution.
        /// </summary>
        /// <value>The invalid solution.</value>
        public static RecaptchaResponse InvalidSolution { get; set; }

        /// <summary>
        ///     Gets or sets the recaptcha not reachable.
        /// </summary>
        /// <value>The recaptcha not reachable.</value>
        public static RecaptchaResponse RecaptchaNotReachable { get; set; }

        /// <summary>
        ///     Gets or sets whether valid.
        /// </summary>
        /// <value>Whether valid.</value>
        public static RecaptchaResponse Valid { get; set; }

        /// <summary>
        ///     Gets the error code.
        /// </summary>
        /// <value>The error code.</value>
        public string ErrorCode { get; private set; }

        /// <summary>
        ///     Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object"/> to compare with this instance.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var other = (RecaptchaResponse)obj;
            return other != null && (other.IsValid == this.IsValid && other.ErrorCode == this.ErrorCode);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.IsValid.GetHashCode() ^ this.ErrorCode.GetHashCode();
        }

        #endregion
    }
}