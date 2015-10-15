using Perspex.Media.TextFormatting;

namespace Perspex.Media.TextFormatting
{
    public abstract class TextRun
    {
        /// <summary>
        /// Reference to character buffer
        /// </summary>
        //public abstract CharacterBufferReference CharacterBufferReference
        //{ get; }


        /// <summary>
        /// Character length
        /// </summary>
        public abstract int Length
        { get; }


        /// <summary>
        /// A set of properties shared by every characters in the run
        /// </summary>
        public abstract TextRunProperties Properties
        { get; }
    }
}
