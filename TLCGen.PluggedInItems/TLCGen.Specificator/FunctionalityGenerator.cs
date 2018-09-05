﻿using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using TLCGen.Extensions;
using TLCGen.Models;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace TLCGen.Specificator
{
    public static class FunctionalityGenerator
    {
        public static string ToCustomString(this bool value)
        {
            return value ? (string)Texts["Generic_True"] : (string)Texts["Generic_False"];
        }

        public static string GetKruisingNaam(ControllerModel c)
        {
            return $"{c.Data.Straat1}" +
                (string.IsNullOrWhiteSpace(c.Data.Straat2) ? "" : $" - {c.Data.Straat2} ");
        }

        private static ResourceDictionary _texts;
        public static ResourceDictionary Texts
        {
            get
            {
                if (_texts == null)
                {
                    var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    using (var stream = assembly.GetManifestResourceStream("TLCGen.Specificator.Resources.SpecificatorTexts.xaml"))
                    {
                        _texts = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(stream);
                    }
                }
                return _texts;
            }
        }

        public static void AddHeaderTextsToDocument(WordprocessingDocument doc, SpecificatorDataModel model, ControllerDataModel data)
        {
            foreach (var h in doc.MainDocumentPart.HeaderParts)
            {
                foreach (var t in h.RootElement.Descendants<Paragraph>().SelectMany(x => x.Descendants<Run>()).SelectMany(x => x.Descendants<Text>()))
                {
                    if (t.Text.Contains("FIRSTPAGEHEADER")) t.Text = "";
                    if (t.Text.Contains("HEADER")) t.Text = $"Functionele specificatie {data.Naam} ({data.Straat1}{(!string.IsNullOrWhiteSpace(data.Straat2) ? " - " + data.Straat2 : "")}, {data.Stad})";
                }
            }
            foreach (var f in doc.MainDocumentPart.FooterParts)
            {
                foreach (var r in f.RootElement.Descendants<Run>())
                {
                    var fp = false;
                    foreach (var t in r.Descendants<Text>())
                    {
                        if (t.Text.Contains("FIRSTPAGEFOOTER"))
                        {
                            t.Text = "";
                            fp = true;
                        }
                    }
                    if (fp)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Organisatie))
                        {
                            r.AppendChild(new Text(model.Organisatie));
                            r.AppendChild(new Break());
                        }
                        if (!string.IsNullOrWhiteSpace(model.Straat))
                        {
                            r.AppendChild(new Text(model.Straat));
                            r.AppendChild(new Break());
                        }
                        var t = "";
                        if (!string.IsNullOrWhiteSpace(model.Postcode))
                        {
                            t = model.Postcode;
                        }
                        if (!string.IsNullOrWhiteSpace(model.Stad))
                        {
                            t = t + " " + model.Stad;
                        }
                        if (t != "")
                        {
                            r.AppendChild(new Text(t));
                            r.AppendChild(new Break());
                        }
                        if (!string.IsNullOrWhiteSpace(model.TelefoonNummer))
                        {
                            r.AppendChild(new Text($"Telefoon: {model.TelefoonNummer}"));
                            r.AppendChild(new Break());
                        }
                        if (!string.IsNullOrWhiteSpace(model.EMail))
                        {
                            r.AppendChild(new Text($"E-mail: {model.EMail}"));
                        }
                    }
                }
            }
        }

        public static string ReplaceHolders(string text, ControllerModel c, SpecificatorDataModel model)
        {
            var t = text;
            t = Regex.Replace(t, "__KR__", c.Data.Naam);
            t = Regex.Replace(t, "__STAD__", c.Data.Stad);
            t = Regex.Replace(t, "__STRAAT1__", c.Data.Straat1);
            t = Regex.Replace(t, "__STRAAT2__", c.Data.Straat2);
            return t;
        }

        public static List<OpenXmlCompositeElement> GetFirstPage(ControllerDataModel data)
        {
            var par1 = new Paragraph();
            var run = par1.AppendChild(new Run());
            run.AppendChild(new Text($"{Texts["Title_Document"]} {data.Naam}"));
            OpenXmlHelper.ApplyStyleToParagraph(par1, "Title");

            var par2 = new Paragraph();
            run = par2.AppendChild(new Run());
            run.AppendChild(new Text($"{data.Straat1}{(!string.IsNullOrWhiteSpace(data.Straat2) ? " - " + data.Straat2 : "")}"));
            OpenXmlHelper.ApplyStyleToParagraph(par2, "Subtitle");

            var par3 = new Paragraph();
            run = par3.AppendChild(new Run());
            run.AppendChild(new Text($"{data.Stad}"));
            OpenXmlHelper.ApplyStyleToParagraph(par3, "Subtitle");

            return new List<OpenXmlCompositeElement> { par1, par2, par3 };
        }

        public static List<OpenXmlCompositeElement> GetVersionControl(ControllerDataModel data)
        {
            var title = OpenXmlHelper.GetTextParagraph($"{Texts["Title_Versioning"]}", "Subtitle");

            var table = new Table();

            TableProperties props = new TableProperties(
                new TableBorders(
                new TopBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new BottomBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new LeftBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new RightBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new InsideHorizontalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 },
                new InsideVerticalBorder { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 1 }),
                new TableWidth() { Type = TableWidthUnitValues.Pct, Width = $"{100 * 50}" });

            table.AppendChild(props);

            foreach (var v in data.Versies)
            {
                table.Append(OpenXmlHelper.GetTableRow(
                    new[] { v.Datum.ToShortDateString(), v.Versie, v.Ontwerper, v.Commentaar },
                    new[] { 14, 14, 14, 58 }));
            }

            return new List<OpenXmlCompositeElement> { title, table };
        }

        public static void GetIntroChapter(WordprocessingDocument doc, ControllerModel c, SpecificatorDataModel model)
        {
            var body = doc.MainDocumentPart.Document.Body;

            body.Append(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Intro"]}", 1));

            var text = 
                $"Dit document beschrijft de functionele eisen aangaande de verkeersregelinstallatie (VRI) op het " +
                $"kruispunt " +
                GetKruisingNaam(c) +
                (Regex.IsMatch(c.Data.Stad, @"^(g|G)emeente") ? $"in de {c.Data.Stad}" : $"te {c.Data.Stad} ({c.Data.Naam}).");
            body.Append(OpenXmlHelper.GetTextParagraph(text));

            if (!string.IsNullOrEmpty(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName))
            {
                var path = Path.GetDirectoryName(DataAccess.TLCGenControllerDataProvider.Default.ControllerFileName);
                var file = Path.Combine(path, (Regex.IsMatch(c.Data.BitmapNaam, @"\.(bmp|BMP)$") ? c.Data.BitmapNaam : c.Data.BitmapNaam + ".bmp"));
                if (File.Exists(file))
                {
                    OpenXmlHelper.AddImageToBody(doc, file);
                }
            }

            var sb = new StringBuilder();

            text = $"Het kruispunt {GetKruisingNaam(c)} wordt schematisch weergegeven in de bovenstaande figuur.";
            //    $"Alle bewegingen worden conflictvrij afgehandeld. De maximumsnelheid op alle naderrichtingen bedraagt 50 km / h. De verkeersregeling dient aan de volgende eisen te voldoen: • Veilige situatie op het kruispunt. • Goede en efficiënte verkeersafwikkeling. • Logische en acceptabele situatie voor weggebruikers. In het regelprogramma dienen in het commentaar alle functionele beschrijvingen van alle parameters, tijden, schakelaars en dergelijke opgenomen te worden.";

            body.Append(OpenXmlHelper.GetTextParagraph(text));

            body.Append(OpenXmlHelper.GetChapterTitleParagraph("Algemene instellingen", 2));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Optie"],
                    (string)Texts["Generic_Instelling"]
                }
            };
            l.Add(new List<string> { (string)Texts["Generic_Fasebewaking"], c.Data.Fasebewaking.ToString() });
            l.Add(new List<string> { (string)Texts["Generic_CCOLVersie"], c.Data.CCOLVersie.GetDescription() });
            if (c.Data.CCOLVersie <= Models.Enumerations.CCOLVersieEnum.CCOL8)
            {
                l.Add(new List<string> { (string)Texts["Generic_TypeVLOG"], c.Data.VLOGType.GetDescription() });
            }
            else
            {
                l.Add(new List<string> { (string)Texts["Generic_VLOGToepassen"], c.Data.VLOGSettings.VLOGToepassen.ToCustomString() });
                l.Add(new List<string> { (string)Texts["Generic_VLOGVersie"], c.Data.VLOGSettings.VLOGVersie.GetDescription() });
                l.Add(new List<string> { (string)Texts["Generic_VLOGLoggingMode"], c.Data.VLOGSettings.LOGPRM_VLOGMODE.GetDescription() });
                l.Add(new List<string> { (string)Texts["Generic_VLOGMonitoringMode"], c.Data.VLOGSettings.MONPRM_VLOGMODE.GetDescription() });
            }
            l.Add(new List<string> { (string)Texts["Generic_TypeKWC"], c.Data.KWCType.GetDescription() });
            l.Add(new List<string> { (string)Texts["Generic_ExtraMeevInWG"], c.Data.ExtraMeeverlengenInWG.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_AanstWaitsig"], c.Data.AansturingWaitsignalen.GetDescription() });
            l.Add(new List<string> { (string)Texts["Generic_SegmDisplay"], c.Data.SegmentDisplayType.GetDescription() });
            l.Add(new List<string> { (string)Texts["Generic_UsPerML"], c.Data.UitgangPerModule.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_FixatieMogelijk"], c.Data.FixatieMogelijk.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_BijkomenFixatie"], c.Data.BijkomenTijdensFixatie.ToCustomString() });
            l.Add(new List<string> { (string)Texts["Generic_TypeGroentijden"], c.Data.TypeGroentijden.GetDescription() });
            body.Append(OpenXmlHelper.GetTable(l));
        }

        public static List<OpenXmlCompositeElement> GetFasenChapter(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Fasen"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Functies"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Type",
                    "Rijstroken",
                    "Vaste aanvraag",
                    "Wachtgroen",
                    "Meeverlengen",
                    "Wachttijd voorspellers",
                    "Aantal detectoren"
                }
            };
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.Type.GetDescription(),
                    fc.AantalRijstroken.ToString(),
                    fc.VasteAanvraag.GetDescription(),
                    fc.Wachtgroen.GetDescription(),
                    fc.Meeverlengen.GetDescription(),
                    fc.WachttijdVoorspeller == true ? "X" : "-",
                    fc.Detectoren.Count.ToString()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Fasen_Tijden"], styleid: "Caption"));
            l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Vastgroen",
                    "Garantiegroen",
                    "Minimum garantiegroen",
                    "Garantierood",
                    "Minimum garantierood",
                    "Geel",
                    "Minimum geel",
                    "Kopmax"
                }
            };
            // TODO: veiligheidsgroen
            foreach (var fc in c.Fasen)
            {
                l.Add(new List<string>
                {
                    fc.Naam,
                    fc.TFG.ToString(),
                    fc.TGG.ToString(),
                    fc.TGG_min.ToString(),
                    fc.TRG.ToString(),
                    fc.TRG_min.ToString(),
                    fc.TGL.ToString(),
                    fc.TGL_min.ToString(),
                    fc.Detectoren.Any(x => (x.Verlengen == Models.Enumerations.DetectorVerlengenTypeEnum.Kopmax)) ? fc.Kopmax.ToString() : "-" });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        public static List<OpenXmlCompositeElement> GetDetectorenChapter(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren"]}", 2));

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_Functies"], styleid: "Caption"));
            var l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"],
                    "Fase",
                    "Type",
                    "Aanvraag",
                    "Verlengen",
                    "Aanvraag direct",
                    "Wachtlicht",
                    "Rijstrook",
                    "Aanvraag bij storing",
                    "Veiligheidsgroen"
                }
            };
            foreach (var fc in c.Fasen)
            foreach (var d in fc.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    fc.Naam,
                    d.Type.GetDescription(),
                    d.Aanvraag.GetDescription(),
                    d.Verlengen.GetDescription(),
                    d.AanvraagDirect.ToString(),
                    d.Wachtlicht.ToCustomString(),
                    d.Rijstrook.ToString(),
                    d.AanvraagBijStoring.GetDescription(),
                    d.VeiligheidsGroen.GetDescription(),
                });
            }
            foreach (var d in c.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    "-",
                    d.Type.GetDescription(),
                    d.Aanvraag.GetDescription(),
                    d.Verlengen.GetDescription(),
                    d.AanvraagDirect.ToCustomString(),
                    d.Wachtlicht.ToCustomString(),
                    d.Rijstrook.ToString(),
                    d.AanvraagBijStoring.GetDescription(),
                    d.VeiligheidsGroen.GetDescription()
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Detectoren_Tijden"], styleid: "Caption"));
            l = new List<List<string>>
            {
                new List<string>
                {
                    (string)Texts["Generic_Detector"],
                    "Fase",
                    "Bezettijd",
                    "Hiaattijd",
                    "Ondergedrag",
                    "Bovengedrag",
                    "Flutter tijd",
                    "Flutter counter"
                }
            };
            foreach (var fc in c.Fasen)
            foreach (var d in fc.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    fc.Naam,
                    d.TDB.ToString(),
                    d.TDH.ToString(),
                    d.TOG.ToString(),
                    d.TBG.ToString(),
                    d.TFL.ToString(),
                    d.CFL.ToString(),
                });
            }
            foreach (var d in c.Detectoren)
            {
                l.Add(new List<string>
                {
                    d.Naam,
                    "-",
                    d.TDB.ToString(),
                    d.TDH.ToString(),
                    d.TOG.ToString(),
                    d.TBG.ToString(),
                    d.TFL.ToString(),
                    d.CFL.ToString(),
                });
            }
            items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));

            return items;
        }

        // TODO: tabel met ingangen (indien aanwezig)
        // TODO: tabel met selectieve detectie (indien aanwezig)
        // TODO: hoofdstuk met omgang met detectiestoring (opm: de optie 'aanvraag bij storing' per detector zit in tabel met detectoren) 

        public static List<OpenXmlCompositeElement> GetRichtingGevoeligChapter(ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();
            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Detectoren"]}", 2));

            if (c.RichtingGevoeligeAanvragen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Richtinggevoelig_Aanvragen"], styleid: "Caption"));
                var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase",
                    "Van",
                    "Naar",
                    "Maximaal tijdsverschil",
                    "Reset aanvraag",
                    "Reset tijd"
                }
            };
                foreach (var rga in c.RichtingGevoeligeAanvragen)
                {
                    l.Add(new List<string>
                {
                    rga.FaseCyclus,
                    rga.VanDetector,
                    rga.NaarDetector,
                    rga.MaxTijdsVerschil.ToString(),
                    rga.ResetAanvraag.ToCustomString(),
                    rga.ResetAanvraag ? rga.ResetAanvraagTijdsduur.ToString() : "-"
                });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }

            if (c.RichtingGevoeligVerlengen.Any())
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Richtinggevoelig_Verlengen"], styleid: "Caption"));
                var l = new List<List<string>>
            {
                new List<string>
                {
                    "Fase",
                    "Van",
                    "Naar",
                    "Type verlengen",
                    "Maximaal tijdsverschil",
                    "Verleng tijd"
                }
            };
                foreach (var rgv in c.RichtingGevoeligVerlengen)
                {
                    l.Add(new List<string>
                {
                    rgv.FaseCyclus,
                    rgv.VanDetector,
                    rgv.NaarDetector,
                    rgv.TypeVerlengen.ToString(),
                    rgv.MaxTijdsVerschil.ToString(),
                    rgv.VerlengTijd.ToString(),
                });
                }
                items.Add(OpenXmlHelper.GetTable(l, firstRowVerticalText: true));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetPeriodenChapter(ControllerModel c)
        {
            var title = OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Perioden"]}", 2);
            var tabelTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_Perioden"], styleid: "Caption");

            var l = new List<List<string>>
            {
                new List<string> { "Periode", "Start", "Einde", "Dagtype" }
            };
            foreach (var p in c.PeriodenData.Perioden)
            {
                l.Add(new List<string> { p.Naam, p.StartTijd.ToString(@"hh\:mm"), p.EindTijd.ToString(@"hh\:mm"), p.DagCode.ToString() });
            }
            var table = OpenXmlHelper.GetTable(l);

            return new List<OpenXmlCompositeElement> { title, tabelTitle, table };
        }

        public static List<OpenXmlCompositeElement> GetModulenChapter(WordprocessingDocument doc, ControllerModel c)
        {
            var items = new List<OpenXmlCompositeElement>();

            items.Add(OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Modulen"]}", 2));
            items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Modulen"], styleid: "Caption"));

            var l = new List<List<string>>
            {
                new List<string> { (string)Texts["Generic_Modulen"], (string)Texts["Generic_Fasen"] }
            };
            c.ModuleMolen.Modules.ForEach(m => l.Add(new List<string> { m.Naam, m.Fasen.Select(x => x.FaseCyclus).Aggregate((y, z) => y + ", " + z) }));
            items.Add(OpenXmlHelper.GetTable(l));

            items.Add(OpenXmlHelper.GetTextParagraph($"Wanneer er geen aanvragen zijn wacht de regeling in module {c.ModuleMolen.WachtModule}."));

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph(
                    $"De regeling is voorzien van langswachtende alternatief. " +
                    $"Per richting is instelbaar of die alternatief mag realiseren, " +
                    $"welke ruimte er minimaal nog moet om dat toe te staan, en welke groentijd minimaal wordt gegeven bij een alternatieve realisatie."));
            }
            items.Add(OpenXmlHelper.GetTextParagraph(
                $"Wanneer daar ruimte voor is, kunnen richtingen primair vooruit realiseren. " +
                $"Het aantal {Texts["Generic_modulen"]} dat vooruit gerealiseerd kan worden is instelbaar." +
                $"Vooruit realiseren gaat voor op alternatief realiseren: wanneer beide mogelijk zijn zal een richting vooruit komen."));

            if (c.ModuleMolen.LangstWachtendeAlternatief)
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_VooruitAltInst"], styleid: "Caption"));
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"],
                        "Aantal modulen vooruit",
                        "Alternatief toegestaan",
                        "Alternatieve ruimte",
                        "Alternatieve groentijd"
                    }
                };
                c.ModuleMolen.FasenModuleData.ForEach(x => l.Add(new List<string>
                {
                    x.FaseCyclus,
                    x.ModulenVooruit.ToString(),
                    x.AlternatiefToestaan.ToCustomString(),
                    x.AlternatieveRuimte.ToString(),
                    x.AlternatieveGroenTijd.ToString()
                }));
                items.Add(OpenXmlHelper.GetTable(l));
            }
            else if (c.ModuleMolen.Modules.Any(x => x.Fasen.Any(x2 => x2.Alternatieven.Any())))
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_Vooruit"], styleid: "Caption"));
                var lt = new List<string>
                {
                    (string)Texts["Generic_Fase"],
                    "Aantal modulen vooruit"
                };
                //foreach(var m in c.ModuleMolen.Modules)
                //{
                //    lt.Add("Alt. groentijd onder " + m.Naam);
                //}
                l = new List<List<string>>
                {
                    lt
                };
                c.ModuleMolen.FasenModuleData.ForEach(x =>
                {
                    var nl = new List<string>
                    {
                        x.FaseCyclus,
                        x.ModulenVooruit.ToString()
                    };
                    //foreach (var m in c.ModuleMolen.Modules)
                    //{
                    //    var t = "-";
                    //    var mfc = m.Fasen.FirstOrDefault(x2 => x2.Alternatieven.Any(x3 => x3.FaseCyclus == x.FaseCyclus));
                    //    if(mfc != null)
                    //    {
                    //        var afc = mfc.Alternatieven.First(x2 => x2.FaseCyclus == x.FaseCyclus);
                    //        t = afc.AlternatieveGroenTijd.ToString();
                    //    }
                    //    nl.Add(t);
                    //}
                    l.Add(nl);
                });
                items.Add(OpenXmlHelper.GetTable(l));

                items.Add(OpenXmlHelper.GetTextParagraph($"De volgende {(string)Texts["Generic_fasen"]} kunnen alternatief realiseren onder dekking van een andere {(string)Texts["Generic_fase"]} in cyclisch verlenggroen (CV)."));

                var il = new List<string>();
                foreach (var m in c.ModuleMolen.Modules)
                { foreach (var f in m.Fasen.Where(x2 => x2.Alternatieven.Any()))
                    {
                        foreach (var a in f.Alternatieven)
                        {
                            il.Add($"In module {m.Naam}: {a.FaseCyclus} onder dekking van {f.FaseCyclus} (groentijd: {a.AlternatieveGroenTijd})");
                        }
                    }
                }
                items.AddRange(OpenXmlHelper.GetBulletList(doc, il));
            }
            else
            {
                items.Add(OpenXmlHelper.GetTextParagraph((string)Texts["Table_VooruitAltInst"], styleid: "Caption"));
                l = new List<List<string>>
                {
                    new List<string>
                    {
                        (string)Texts["Generic_Fase"],
                        "Aantal modulen vooruit"
                    }
                };
                c.ModuleMolen.FasenModuleData.ForEach(x => l.Add(new List<string>{ x.ModulenVooruit.ToString() }));
                items.Add(OpenXmlHelper.GetTable(l));
            }

            return items;
        }

        public static List<OpenXmlCompositeElement> GetGroentijdenChapter(ControllerModel c)
        {
            var title = OpenXmlHelper.GetChapterTitleParagraph($"{Texts["Title_Groentijden"]}", 2);

            OpenXmlCompositeElement tableTitle = null;
            switch (c.Data.TypeGroentijden)
            {
                case Models.Enumerations.GroentijdenTypeEnum.MaxGroentijden:
                    tableTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_MaxGroentijden"], styleid: "Caption");
                    break;
                case Models.Enumerations.GroentijdenTypeEnum.VerlengGroentijden:
                    tableTitle = OpenXmlHelper.GetTextParagraph((string)Texts["Table_VerlGroentijden"], styleid: "Caption");
                    break;
            }

            var l = new List<List<string>>();
            var l1 = new List<string> { (string)Texts["Fase"] };
            foreach (var set in c.GroentijdenSets)
            {
                l1.Add(set.Naam);
            }
            l.Add(l1);
            foreach (var fc in c.Fasen)
            {
                var l2 = new List<string> { fc.Naam };
                foreach (var set in c.GroentijdenSets)
                {
                    var f = set.Groentijden.FirstOrDefault(x => x.FaseCyclus == fc.Naam);
                    if (f != null)
                    {
                        l2.Add(f.Waarde.HasValue ? f.Waarde.Value.ToString() : "-");
                    }
                    else
                    {
                        l2.Add("-");
                    }
                }
                l.Add(l2);
            }
            var table = OpenXmlHelper.GetTable(l);

            return new List<OpenXmlCompositeElement> { title, tableTitle, table };
        }


    }
}