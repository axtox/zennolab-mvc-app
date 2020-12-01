using System;
using System.ComponentModel.DataAnnotations;

namespace ZennoLab.Models {

    public enum PictureAnswers { None, FileName, SeparateFile }

    public class UserDataSetViewModel {

        [MinLength(4)]
        [MaxLength(8)]
        public string Name {get;set;}

        [DataType(DataType.Date)]
        public DateTime Date {get;set;}

        public bool HasCyrillicSymbols {get;set;}

        public bool HasLatinSymbols {get;set;}

        public bool HasDigits {get;set;}

        public bool HasSpecialSymbols {get;set;}

        public bool IsCaseSensitive {get;set;}

        public PictureAnswers AnswersSource {get;set;}
    }
}