namespace SV21T1020206.DataLayers
{
    public  interface ISimpleQueryDAL<T> where T : class
    {
        /// <summary>
        /// lay toan bo du lieu 
        /// </summary>
        /// <returns></returns>
        List<T> List();
    }
}
