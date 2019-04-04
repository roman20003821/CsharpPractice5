namespace CSharpPractice5.Model
{
    internal class Module
    {

        #region fields

        private string _fileName;
        private string _moduleName;

        #endregion

        #region properties

        public string FileName
        {
            get
            {
                return _fileName;
            }
        }

        public string ModuleName
        {
            get
            {
                return _moduleName;
            }
        }

        #endregion

        internal Module(string moduleName, string fileName)
        {
            _fileName = moduleName;
            _moduleName = fileName;
        } 
    }
}
