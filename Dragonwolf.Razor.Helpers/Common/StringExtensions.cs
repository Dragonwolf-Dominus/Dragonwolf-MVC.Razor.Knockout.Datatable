using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dragonwolf.Razor.Helpers.Common
{
    public static class StringExtensions
    {
        #region Constants
        private const string RemplacementLeftBraquetDiacritic = "##-##";
        private const string RemplacementRigthBraquetDiacritic = "--#--";
        private const string LeftBraquetDiacritic = "{ ";
        private const string RigthBraquetDiacritic = " }";
        #endregion

        #region Public Methods
        public static string SpecialFormat(this string sourceString, params object[] args)
        {
            var result = sourceString;

            result = result.Replace(LeftBraquetDiacritic, RemplacementLeftBraquetDiacritic).Replace(RigthBraquetDiacritic, RemplacementRigthBraquetDiacritic);

            result = string.Format(result, args: args);

            result = result.Replace(RemplacementLeftBraquetDiacritic, LeftBraquetDiacritic).Replace(RemplacementRigthBraquetDiacritic, RigthBraquetDiacritic);

            return result;
        }
        #endregion
    }
}
