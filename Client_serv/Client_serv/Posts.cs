//------------------------------------------------------------------------------
// <auto-generated>
//    Этот код был создан из шаблона.
//
//    Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//    Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Client_serv
{
    using System;
    using System.Collections.Generic;
    
    public partial class Posts
    {
        public int PostID { get; set; }
        public string Post { get; set; }
    
        public virtual Workers Workers { get; set; }
    }
}
