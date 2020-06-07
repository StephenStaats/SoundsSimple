using System;
using System.Collections.Generic;
using System.Text;

namespace SoundsSimple.Models
{


   public static class SoundFileData
   {
      public static IList<SoundFile> SoundFiles { get; private set; }

      static SoundFileData()
      {
         SoundFiles = new List<SoundFile>();

         SoundFiles.Add(new SoundFile
         {
            Name = "Baboon",
            Location = "Africa & Asia",
            Details = "Baboons are African and Arabian Old World monkeys belonging to the genus Papio, part of the subfamily Cercopithecinae.",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/fc/Papio_anubis_%28Serengeti%2C_2009%29.jpg/200px-Papio_anubis_%28Serengeti%2C_2009%29.jpg"
         });

         SoundFiles.Add(new SoundFile
         {
            Name = "Capuchin Monkey",
            Location = "Central & South America",
            Details = "The capuchin monkeys are New World monkeys of the subfamily Cebinae. Prior to 2011, the subfamily contained only a single genus, Cebus.",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/40/Capuchin_Costa_Rica.jpg/200px-Capuchin_Costa_Rica.jpg"
         });

         SoundFiles.Add(new SoundFile
         {
            Name = "Blue Monkey",
            Location = "Central and East Africa",
            Details = "The blue monkey or diademed monkey is a species of Old World monkey native to Central and East Africa, ranging from the upper Congo River basin east to the East African Rift and south to northern Angola and Zambia",
            ImageUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/8/83/BlueMonkey.jpg/220px-BlueMonkey.jpg"
         });

      }
   }

}
