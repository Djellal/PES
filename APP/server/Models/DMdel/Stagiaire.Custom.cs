using System.ComponentModel.DataAnnotations.Schema;

namespace Pes.Models.DMdel
{
  /// <summary>
  /// Regles metier complementaires du modele genere Stagiaire.
  /// Ce fichier est un partial : il n'est pas ecrase par la regeneration Radzen.
  /// </summary>
  public partial class Stagiaire
  {
    /// <summary>Seuil de validation, en fraction de la note (0.5 = 50 %).</summary>
    public const double SeuilAttestation = 0.5;

    /// <summary>
    /// L'attestation n'est accessible que si la note finale ET la note de cours
    /// atteignent le seuil. Indispensable : sans le test sur la note de cours,
    /// une tres bonne note de jury maskerait une evaluation continue tres basse.
    /// </summary>
    [NotMapped]
    public bool EstAdmissibleAttestation
    {
      get
      {
        return NoteFinale >= SeuilAttestation && Note >= SeuilAttestation;
      }
    }
  }
}
