﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Stolons.Models
{
    public class Producer : User
    {
        [Display(Name = "Raison sociale")]
        public string CompanyName { get; set; }
        [Display(Name = "Superficie en m²")]
        public int Area { get; set; }

        private IList<string> _ExploitationPictures;
        [Display(Name = "Galerie d'exploitations")]
        [NotMapped]
        public IList<string> ExploitationPictures
        {
            get
            {

                return _ExploitationPictures;

            }
            set
            {
                _ExploitationPictures = value;
            }
        }
        public string ExploitationPicuresSerialized
        {
            get
            {
                return String.Join(";", _ExploitationPictures);
            }
            set
            {
                _ExploitationPictures = Tools.SerializeStringToList(value);
             }
        }

        [Display(Name = "Production")]
        public string Production { get; set; }
        [Display(Name = "Texte libre")]
        public string OpenText { get; set; }
        [Display(Name = "Année d'installation")]
        public DateTime StartDate { get; set; }
        [Display(Name = "Lien vers le site web")]
        public string WebSiteLink { get; set; }
        [Display(Name = "Factures")]
        public List<Bill> Bills { get; set; }
    }
}
