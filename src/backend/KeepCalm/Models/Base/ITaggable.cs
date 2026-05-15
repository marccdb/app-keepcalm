using System.Collections.Generic;
using KeepCalm.Models.Entities;

namespace KeepCalm.Models.Base
{
    public interface ITaggable
    {
        public List<string> Tags { get; set; }
    }
}
