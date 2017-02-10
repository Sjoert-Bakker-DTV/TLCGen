﻿using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TLCGen.Messaging;
using TLCGen.Messaging.Requests;
using TLCGen.Models;
using TLCGen.Plugins;

namespace TLCGen.Integrity
{
    public static class IntegrityChecker
    {
        /// <summary>
        /// Checks the integrity of the data in the instance of ControllerModel that is parsed in
        /// </summary>
        /// <param name="Controller">The instance of ControllerModel to check for integrity</param>
        /// <returns></returns>
        public static string IsControllerDataOK(ControllerModel _Controller)
        {
            string s = IsConflictMatrixOK(_Controller);
            if (!string.IsNullOrEmpty(s))
            {
                return s;
            }
            return null;
        }

        /// <summary>
        /// Checks if the ConflictMatrix is symmetrical.
        /// </summary>
        /// <returns>null if succesfull, otherwise a string stating the first error found.</returns>
        public static string IsConflictMatrixOK(ControllerModel _Controller)
        {
            // Request to process all synchronisation data from matrix to model
            Messenger.Default.Send(new ProcessSynchronisationsRequest());

            // Loop all conflicts
            foreach (ConflictModel cm1 in _Controller.InterSignaalGroep.Conflicten)
            {
                bool Found = false;
                foreach (ConflictModel cm2 in _Controller.InterSignaalGroep.Conflicten)
                {
                    if (cm1.FaseVan == cm2.FaseNaar && cm1.FaseNaar == cm2.FaseVan)
                    {
                        Found = true;
                        switch (cm1.SerializedWaarde)
                        {
                            case "FK":
                                if (cm2.SerializedWaarde != "FK")
                                    return "Conflict matrix niet symmetrisch:\nFK van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " maar niet andersom.";
                                break;
                            case "GK":
                                if (cm2.SerializedWaarde != "GK" && cm2.SerializedWaarde != "GKL")
                                    return "Conflict matrix niet symmetrisch:\nGK van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " maar niet andersom.";
                                continue;
                            case "GKL":
                                if (cm2.SerializedWaarde != "GK")
                                    return "Conflict matrix niet symmetrisch:\nGKL van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " maar andersom geen GK.";
                                continue;
                            default:
                                int co;
                                if (Int32.TryParse(cm1.SerializedWaarde, out co))
                                {
                                    // Check against guaranteed timings
                                    if (_Controller.Data.GarantieOntruimingsTijden)
                                    {
                                        if (cm1.GarantieWaarde == null)
                                            return "Ontbrekende garantie ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + ".";
                                        else if (co < cm1.GarantieWaarde)
                                            return "Ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " lager dan garantie ontruimmingstijd.";
                                    }

                                    if (Int32.TryParse(cm2.SerializedWaarde, out co))
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        return "Conflict matrix niet symmetrisch:\nwaarde van " + cm2.FaseVan + " naar " + cm2.FaseNaar + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                    }
                                }
                                else
                                {
                                    return "Conflict matrix niet symmetrisch:\nwaarde van " + cm2.FaseVan + " naar " + cm2.FaseNaar + " ontbrekend of onjuist (niet numeriek, FK, GK of GKL).";
                                }
                        }

                        if (_Controller.Data.GarantieOntruimingsTijden)
                        {
                            int out1;

                            if (Int32.TryParse(cm1.SerializedWaarde, out out1))
                            {
                                if (cm1.GarantieWaarde != null && cm1.GarantieWaarde >= 0)
                                {
                                    if (out1 < cm1.GarantieWaarde)
                                    {
                                        return "Ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " lager dan garantie ontruimmingstijd.";
                                    }
                                }
                                else
                                {
                                    return "Ontbrekende garantie ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + ".";
                                }
                            }
                            else if (cm1.GarantieWaarde != null && cm1.GarantieWaarde >= 0)
                            {
                                return "Ontbrekende ontruimingstijd van " + cm1.FaseVan + " naar " + cm1.FaseNaar + ".";
                            }
                        }
                    }
                    if (Found) break;
                }
                if (!Found)
                {
                    return "Conflict matrix niet symmetrisch:\nconflict van " + cm1.FaseVan + " naar " + cm1.FaseNaar + " ontbreekt.";
                }
            }
            return null;
        }

        /// <summary>
        /// Checks if an element's Name property is unique accross the ControllerModel
        /// </summary>
        /// <param name="naam">The Name property to check</param>
        /// <returns>True if unique, false if not</returns>
        public static bool IsElementNaamUnique(ControllerModel _Controller, string naam)
        {
            // Check fasen
            foreach (FaseCyclusModel fcvm in _Controller.Fasen)
            {
                if (fcvm.Naam == naam)
                    return false;
            }

            // Check detectie
            foreach (FaseCyclusModel fcvm in _Controller.Fasen)
            {
                foreach (DetectorModel dvm in fcvm.Detectoren)
                {
                    if (dvm.Naam == naam)
                        return false;
                }
            }
            foreach (DetectorModel dvm in _Controller.Detectoren)
            {
                if (dvm.Naam == naam)
                    return false;
            }

            // Check perioden
            foreach(var p in _Controller.PeriodenData.Perioden)
            {
                if (p.Naam == naam)
                    return false;
            }

            // Check plugins
            foreach(var pl in TLCGenPluginManager.Default.ApplicationParts)
            {
                if ((pl.Item1 & TLCGenPluginElems.IOElementProvider) == TLCGenPluginElems.IOElementProvider)
                {
                    if (!(pl as ITLCGenElementProvider).IsElementNameUnique(naam))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if an element's VissimName property is unique accross the ControllerModel
        /// </summary>
        /// <param name="vissimnaam">The VissimName property to check</param>
        /// <returns>True if unique, false if not</returns>
        public static bool IsElementVissimNaamUnique(ControllerModel _Controller, string vissimnaam)
        {
            // Check detectie
            foreach (FaseCyclusModel fcvm in _Controller.Fasen)
            {
                foreach (DetectorModel dvm in fcvm.Detectoren)
                {
                    if (dvm.VissimNaam == vissimnaam)
                        return false;
                }
            }
            foreach (DetectorModel dvm in _Controller.Detectoren)
            {
                if (dvm.VissimNaam == vissimnaam)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(ControllerModel _Controller, FaseCyclusModel fcm1, FaseCyclusModel fcm2)
        {
            if (_Controller == null)
                throw new NotImplementedException();

            foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == fcm1.Naam && cm.FaseNaar == fcm2.Naam)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if this phase conflicts with the one parsed
        /// </summary>
        public static bool IsFasenConflicting(ControllerModel _Controller, string define1, string define2)
        {
            if (_Controller == null)
                throw new NotImplementedException();

            foreach (ConflictModel cm in _Controller.InterSignaalGroep.Conflicten)
            {
                if (cm.FaseVan == define1 && cm.FaseNaar == define2)
                    return true;
            }
            return false;
        }
    }
}
