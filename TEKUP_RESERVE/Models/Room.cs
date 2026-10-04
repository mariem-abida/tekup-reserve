namespace TEKUP_RESERVE.Models
{
    public class Room
    {
        public int Id { get; set; }           // Clé primaire

        public string Name { get; set; }      // Nom de la salle : S101, Labo IA...
        public string Type { get; set; }      // TD, TP, Amphi, Labo
        public int Capacity { get; set; }     // Capacité (nombre de personnes)

        public bool HasComputers { get; set; }    // Salle info ?
        public bool HasProjector { get; set; }    // Vidéoprojecteur ?
        public bool HasAudioSystem { get; set; }  // Son ?
    }
}
