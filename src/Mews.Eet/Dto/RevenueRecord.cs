using System;
using System.Security.Cryptography;
using System.Text;
using Mews.Eet.Dto.Identifiers;

namespace Mews.Eet.Dto
{
  public class RevenueRecord
  {
    public RevenueRecord(Identification identification, Revenue revenue, BillNumber billNumber, bool mandatedByMultipleTaxPayers = false, bool isFirstAttempt = true)
    {
      Identifier = Guid.NewGuid();      //musí být verze 4!
      Identification = identification;
      MandatedByMultipleTaxPayers = mandatedByMultipleTaxPayers;
      Revenue = revenue;
      BillNumber = billNumber;
      IsFirstAttempt = isFirstAttempt;
    }

    public Guid Identifier { get; }

    public Identification Identification { get; }

    public bool MandatedByMultipleTaxPayers { get; }

    public Revenue Revenue { get; }

    public BillNumber BillNumber { get; }

    public bool IsFirstAttempt { get; }
  }
}
