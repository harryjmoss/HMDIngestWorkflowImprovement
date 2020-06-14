﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HMDSharepointChecker
{
    class LabelledFile
    {
        public List<string> fileName = new List<String>();
        public List<string> flagStatus = new List<String>();
        public List<string> objectType = new List<String>();
        public List<string> imgLabel = new List<String>();
        public List<string> orderNumber = new List<String>();

        public List<string> frontMatter = new List<String>();
        public List<string> folios = new List<String>();
        public List<string> endFlysheets = new List<String>();
        public List<string> endMatter = new List<String>();

        public LabelledFile()
        { }




    }

    class InputOrderSpreadsheetTools
    {
        

        public static List<List<List<String>>> listAllShelfmarkFilesTIFXML(List<List<String>> sharepointOut, String env)
        {
            bool fError = false;
            List<List<String>> sourceFolderXMLs = new List<List<String>>(); // maybe don't need?
            List<List<List<String>>> allShelfmarkTIFAndLabels = new List<List<List<String>>>();


            for (int i = 1; i < sharepointOut.Count; i++) // need this to skip the first row (titles)
            {
                List<String> item = sharepointOut[i];
                List<String> shelfmarkTIFs = new List<String>();

                List<List<String>> shelfmarkLabels = new List<List<String>>();
                bool validPath = false;
                
                var shelfmark = item[1];

                if (item[5] != "false")
                {
                    string sourceFolder = "";

                    if (string.IsNullOrEmpty(item[6]))
                    {
                        sourceFolder = item[2];
                    }
                    else
                    {
                        sourceFolder = item[6];
                    }

                    // Once you've got sourceFolder, need to get into the actual image folders...

                    // this is a bit of a mess at the moment, sort this out
                    var tifFolder = "";
                    sourceFolder = sourceFolder.TrimEnd('\\');
                    sourceFolder = sourceFolder.ToLower();

                    Console.WriteLine("Source Folder: {0}", sourceFolder);
                    try
                    {
                        if (sourceFolder.ToUpper().ToLower().Contains("tif"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else
                        {

                            var subFolders = Directory.GetDirectories(sourceFolder);
                            foreach (var subFolder in subFolders)
                            {
                                Console.WriteLine("Testing subFolder: {0}", subFolder);
                                if (subFolder.ToUpper().ToLower().Contains("tif"))
                                {
                                    tifFolder = sourceFolder;
                                    Console.WriteLine("Found subfolder for folder {0}", sourceFolder);
                                }
                            }
                        }
                        

                        /*
                        if (sourceFolder.IndexOf("tif", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (sourceFolder.EndsWith("TIF"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (sourceFolder.EndsWith("tif"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (sourceFolder.EndsWith("tiff"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (sourceFolder.EndsWith("TIFF"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (sourceFolder.EndsWith("tiffs"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (sourceFolder.EndsWith("TIFFS"))
                        {
                            tifFolder = sourceFolder;
                        }
                        else if (Directory.Exists(sourceFolder + @"\" + "TIFF"))
                        {
                            tifFolder = sourceFolder + @"\" + "TIFF";

                        }
                        else if (Directory.Exists(sourceFolder + @"\" + "tiff"))
                        {
                            tifFolder = sourceFolder + @"\" + "tiff";

                        }
                        else if (Directory.Exists(sourceFolder + @"\" + "TIFFS"))
                        {
                            tifFolder = sourceFolder + @"\" + "TIFFS";

                        }
                        else if (Directory.Exists(sourceFolder + @"\" + "tiffs"))
                        {
                            tifFolder = sourceFolder + @"\" + "tiffs";

                        }
                        else if (Directory.Exists(sourceFolder + @"\" + "tif"))
                        {
                            tifFolder = sourceFolder + @"\" + "tif";

                        }
                        else if (Directory.Exists(sourceFolder + @"\" + "TIF"))
                        {
                            tifFolder = sourceFolder + @"\" + "TIF";

                        }
                        */
                        if (Directory.Exists(tifFolder))
                        {
                            validPath = true;
                        }
                        else
                        {
                            Console.WriteLine("No folder found for shelfmark {0}", shelfmark);
                            validPath = false;
                           // return null;
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Could not find Tif subfolder for sourcefolder {0}. Exception: {1}", sourceFolder,ex);
                        validPath = false;
                        //return null;
                    }
                    // now got the tiff folder, need to check the list of files that appears

                    // first check it exists:
                    if (validPath)
                    {
                        // Get all tifs
                        DirectoryInfo d = new DirectoryInfo(tifFolder);
                        FileInfo[] Files = d.GetFiles("*.TIF*");

                        // Can then add this to a list of strings
                        string str = "";
                        var numberOfItems = Files.Length; // only do this once per shelfmark
                                                          // do you need this?

                        //shelfmarkTIFs.Add(shelfmark); // why do you need to add the shelfmark? It should be in the filenames

                        // to-do: turn the below stuff into a class of its own
                        shelfmarkLabels = mapFileNameToLabels(Files);
                        // shelfmarkLabels contains 
                        //[0]: filename
                        //[1]: flagStatus
                        //[2]: objectType
                        //[3]: label
                        //[4]: order number

                        // Now write this to a CSV
                        // Not yet...
                       // Assert.IsTrue(writeFileLabelsToCSV(shelfmarkLabels));

                    }// if validPath == true


                    else // so not a valid path!
                    {
                        // really need the class implementation here so I don't have to add 4 nulls...
                        // Old method:
                        /*shelfmarkTIFs.Add(shelfmark);
                        shelfmarkTIFs.Add(null);
                        shelfmarkTIFs.Add(null);
                        shelfmarkTIFs.Add(null);
                        shelfmarkTIFs.Add(null);
                        */

                        // need to build up a list and then add it to shelfmarkLabels
                        var errorList = new List<string> {shelfmark, null,null,null,null};
                        shelfmarkLabels.Add(errorList);

                        continue; // use continue for now, but will need to write out invalid path to a variable at some point
                    }
                }
                else
                {
                    var errorList = new List<string> { shelfmark, null, null, null, null };
                    shelfmarkLabels.Add(errorList);


                    // Old method (deprecated)
                    /*
                    shelfmarkTIFs.Add(shelfmark);
                    shelfmarkTIFs.Add(null);
                    */

                    // Got yourself a shelfmark that needs checking, so obviously things will fail here...
                    continue;
                }
                try
                {
                    allShelfmarkTIFAndLabels.Add(shelfmarkLabels);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error: Could not add shelfmark tif information to the overall list of shelfmarks");
                    return null;
                }
            } // end of the for loop over each shelfmark


            return allShelfmarkTIFAndLabels;
        }
        private static List<List<string>> mapFileNameToLabels(FileInfo[] Files)
        {

            // Order labels will take a couple of sweeps - one to get front and back matter and then another to do a fine sort of the front and back matter
            List<String> shelfmarkLabels = new List<String>();

            List<string> fileNames = Files.Select(x => x.Name).ToList();

            List<String> frontNames = new List<String>();
            List<String> endNames = new List<String>();

            // front matter
            frontNames.Add("fblefr");
            frontNames.Add("fblefv");
            frontNames.Add("fs"); // adding front flysheets

            // end matter
            endNames.Add("fbspi"); // spine is always the last item?
            endNames.Add("fbrigv"); // back cover
            endNames.Add("fbrigr"); // back cover inside
            endNames.Add("fse"); // end flysheets

            // Define regular expressions to search for
            // 'initial' versions perform a looser search
            var frontMatterRegex = new Regex(@"(.)+(((fble)((fv)|(fr)))|((fs)[0-9]+[rv]))\.tif", RegexOptions.IgnoreCase);
            var initialFrontMatterRegex = new Regex(@"(.)+(((fble)((fv)|(fr)))|((fs)[0-9]+(.)+))\.tif", RegexOptions.IgnoreCase);
            
            var folioRegex = new Regex(@"(.)+(f)([0-9])+([rv])\.tif", RegexOptions.IgnoreCase);
            var initialFolioRegex = new Regex(@"(.)+(f)([0-9])+(.)+\.tif", RegexOptions.IgnoreCase);

            var endFlysheetsRegex = new Regex(@"(.)+((fse)[0-9] +[rv])\.tif", RegexOptions.IgnoreCase);
            var initialEndFlysheetsRegex = new Regex(@"(.)+((fse)[0-9]+(.)+)\.tif", RegexOptions.IgnoreCase);

            var endMatterRegex = new Regex(@"(.)+(((fb)((rigv)|(rigr)|(spi))))\.tif", RegexOptions.IgnoreCase);
            var initialEndMatterRegex = new Regex(@"(.)+(((fb)((rigv)|(rigr)|(spi))))\.tif", RegexOptions.IgnoreCase);
            // Sort into front matter, end flysheets, end matter and folios

            // Candidates for each section:
            List<string> cFrontMatter = fileNames.Where(f => initialFrontMatterRegex.IsMatch(f)).ToList();
            List<string> cEndFlysheets = fileNames.Where(f => initialEndFlysheetsRegex.IsMatch(f)).ToList();
            List<string> cEndMatter = fileNames.Where(f => initialEndMatterRegex.IsMatch(f)).ToList();
            List<string> cFolios = fileNames.Where(f => initialFolioRegex.IsMatch(f)).ToList();

            List<List<String>> allFilesSorted = new List<List<String>>(); // make this into a class...
            //LabelledFile allFilesSorted = new LabelledFile();


            List<List<string>> frontMatter = new List<List<String>>();
            List<List<string>> endFlysheets = new List<List<String>>();
            List<List<string>> endMatter = new List<List<String>>();
            List<List<string>> folios = new List<List<String>>();

            if (cFrontMatter.Any() | cFolios.Any() | cEndMatter.Any() | cEndFlysheets.Any())
            {
                // you can be pretty sure its DIPs compliant if you see any titles or any numbered folios

                int DIPSMatches = 0;
                bool FMExists = false;
                bool FOLExists = false;
                bool EFSExists = false;
                bool EMExists = false;
                if (cFrontMatter.Any())
                {
                    DIPSMatches += 1;
                    FMExists = true;
                    foreach (string fname in cFrontMatter)
                    {
                        List<String> fmat = new List<String>();
                        var match = Regex.Match(fname, @"(.)+(((fble)((fv)|(fr)))|((fs)[0-9]+[rv]))\.tif", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            fmat.Add(fname);
                            var fblefr = Regex.Match(fname, @"(.)+(fblefr)\.tif", RegexOptions.IgnoreCase).Success;
                            var fblefv = Regex.Match(fname, @"(.)+(fblefv)\.tif", RegexOptions.IgnoreCase).Success;
                            var fsr = Regex.Match(fname, @"(.)+((fs)[0-9]+[r])\.tif", RegexOptions.IgnoreCase).Success;
                            var fsv = Regex.Match(fname, @"(.)+((fs)[0-9]+[v])\.tif", RegexOptions.IgnoreCase).Success;
                            if (fblefr)
                            {
                                fmat.Add("");
                                fmat.Add("cover");
                                fmat.Add("front cover");
                            }
                            else if (fblefv)
                            {
                                fmat.Add("");
                                fmat.Add("cover");
                                fmat.Add("front cover inside");
                            }
                            else if(fsr)
                            {
                                fmat.Add("");
                                fmat.Add("flysheet");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                string trimmedName = derivedFilename.Trim('f','s');
                                string noZerosName = trimmedName.TrimStart('0');
                                noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                                string flysheetLabelString = "front flysheet" + noZerosName;
                                fmat.Add(flysheetLabelString);
                            }
                            else if (fsv)
                            {
                                fmat.Add("");
                                fmat.Add("flysheet");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                string trimmedName = derivedFilename.Trim('f','s');
                                string noZerosName = trimmedName.TrimStart('0');
                                noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                                string flysheetLabelString = "front flysheet " + noZerosName;
                                fmat.Add(flysheetLabelString);
                            }
                            else
                            {
                                Console.WriteLine("ERROR: SOMETHING HAS GONE BADLY WRONG WITH ORDER & LABEL GEN... CHECK WHAT");
                                string errString = "Unexpected characters in filename. Flag for investigation";
                                fmat.Add(errString);
                                fmat.Add("page");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                fmat.Add(derivedFilename);

                            }
                        }
                        else
                        {
                            string errString = "Unexpected characters in filename. Flag for investigation";
                            fmat.Add(fname);
                            fmat.Add(errString);
                            fmat.Add("page");
                            string[] split = fname.Split('.');
                            string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                            string fileExtension = split.Last(); // tif
                            string[] split2 = shelfmark_filename.Split('_');
                            string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                            string derivedFilename = split2.Last();
                            fmat.Add("derivedFilename");

                        }
                        frontMatter.Add(fmat);
                    }
                }
                if (cFolios.Any())
                {
                    FOLExists = true;
                    DIPSMatches += 1;
                    foreach (string fname in cFolios)
                    {
                        List<String> fols = new List<String>();
                        var match = Regex.Match(fname, @"(.)+(f)([0-9])+([rv])\.tif", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                          
                            fols.Add(fname);
                            var fr = Regex.Match(fname, @"(.)+((f)[0-9]+[r])\.tif", RegexOptions.IgnoreCase).Success;
                            var fv = Regex.Match(fname, @"(.)+((f)[0-9]+[v])\.tif", RegexOptions.IgnoreCase).Success;

                            if (fr)
                            {
                                fols.Add("");
                                fols.Add("page");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                string trimmedName = derivedFilename.Trim('f');
                                string noZerosName = trimmedName.TrimStart('0');
                                noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                                string frString = "folio " + noZerosName;
                                fols.Add(frString);
                                
                            }
                            else if (fv)
                            {
                                fols.Add(""); 
                                fols.Add("page");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                string trimmedName = derivedFilename.Trim('f');
                                string noZerosName = trimmedName.TrimStart('0');
                                noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                                string frString = "folio " + noZerosName;
                                fols.Add(frString);
                            }
                            else
                            {
                                Console.WriteLine("ERROR: Folio outside of common DIPS string range. Investigate");
                                string errString = "Unexpected characters in filename. Flag for investigation";
                                fols.Add(errString);
                                fols.Add("page");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                fols.Add(derivedFilename);
                            }

                        }
                        else
                        {
                            string errString = "Unexpected characters in filename. Flag for investigation";
                            fols.Add(fname);
                            fols.Add(errString);
                            fols.Add("page");
                            string[] split = fname.Split('.');
                            string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                            string fileExtension = split.Last(); // tif
                            string[] split2 = shelfmark_filename.Split('_');
                            string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                            string derivedFilename = split2.Last();
                            fols.Add(derivedFilename);

                        }
                        folios.Add(fols);
                    }
                }
                if (cEndFlysheets.Any())
                {
                    EFSExists = true;
                    DIPSMatches += 1;
                    foreach (string fname in cEndFlysheets)
                    {
                        List<String> efs = new List<String>();
                        var match = Regex.Match(fname, @"(.)+((fse)[0-9] +[rv])\.tif", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            efs.Add(fname);
                            var fser = Regex.Match(fname, @"(.)+((fse)[0-9] +[r])\.tif", RegexOptions.IgnoreCase).Success;
                            var fsev = Regex.Match(fname, @"(.)+((fse)[0-9] +[v])\.tif", RegexOptions.IgnoreCase).Success;
                            if (fser)
                            {
                                efs.Add(""); // error string
                                efs.Add("flysheet");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                string trimmedName = derivedFilename.Trim('f','s','e');
                                string noZerosName = trimmedName.TrimStart('0');
                                noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                                string frString = "back flysheet " + noZerosName;
                                efs.Add(frString);
                            }
                            else if (fsev)
                            {
                                efs.Add(""); // error string
                                efs.Add("flysheet");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                string trimmedName = derivedFilename.Trim('f','s','e');
                                string noZerosName = trimmedName.TrimStart('0');
                                noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                                string frString = "back flysheet " + noZerosName;
                                efs.Add(frString);
                            }
                            else
                            {
                                string errString = "Unexpected characters in filename. Flag for investigation";
                                efs.Add(errString); // error string
                                efs.Add("flysheet");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                efs.Add(derivedFilename);
                            }
                        }
                        else
                        {
                            string errString = "Unexpected characters in filename. Flag for investigation";
                            efs.Add(fname);
                            efs.Add(errString);
                            efs.Add("page");
                            string[] split = fname.Split('.');
                            string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                            string fileExtension = split.Last(); // tif
                            string[] split2 = shelfmark_filename.Split('_');
                            string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                            string derivedFilename = split2.Last();
                            efs.Add(derivedFilename);

                        }
                        endFlysheets.Add(efs);
                    }
                }
                if (cEndMatter.Any())
                {
                    EMExists = true;
                    DIPSMatches += 1;
                    foreach (string fname in cEndMatter)
                    {
                        List<String> ema = new List<String>();
                        var match = Regex.Match(fname, @"(.)+(((fb)((rigv)|(rigr)|(spi))))\.tif", RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            ema.Add(fname);
                            var fbrigr = Regex.Match(fname, @"(.)+(fbrigr)\.tif", RegexOptions.IgnoreCase).Success;
                            var fbrigv = Regex.Match(fname, @"(.)+(fbrigv)\.tif", RegexOptions.IgnoreCase).Success;
                            var fbspi = Regex.Match(fname, @"(.)+(fbspi)\.tif", RegexOptions.IgnoreCase).Success;

                            if (fbrigr)
                            {
                                ema.Add("");
                                ema.Add("cover");
                                ema.Add("back cover inside");
                            }
                            else if (fbrigv)
                            {
                                ema.Add("");
                                ema.Add("cover");
                                ema.Add("back cover");
                            }
                            else if (fbspi)
                            {
                                ema.Add("");
                                ema.Add("cover");
                                ema.Add("spine");
                            }
                            else // no match for any of these 'usual' cases
                            {
                                string errString = "Unexpected characters in filename. Flag for investigation";
                                ema.Add(errString);
                                ema.Add("page");
                                string[] split = fname.Split('.');
                                string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                                string fileExtension = split.Last(); // tif
                                string[] split2 = shelfmark_filename.Split('_');
                                string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                                string derivedFilename = split2.Last();
                                ema.Add(derivedFilename);
                            }


                        }
                        else
                        {
                            string errString = "Unexpected characters in filename. Flag for investigation";
                            ema.Add(fname);
                            ema.Add(errString);
                            ema.Add("page");
                            string[] split = fname.Split('.');
                            string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                            string fileExtension = split.Last(); // tif
                            string[] split2 = shelfmark_filename.Split('_');
                            string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                            string derivedFilename = split2.Last();
                            ema.Add(derivedFilename);
                        }
                        endMatter.Add(ema);
                    }
                }
                // check for anything else that passed through that failed the above checks


                frontMatter.Sort((a, b) => a[0].CompareTo(b[0]));
                folios.Sort((a, b) => a[0].CompareTo(b[0]));
                endFlysheets.Sort((a, b) => a[0].CompareTo(b[0]));
                endMatter.Sort((a, b) => a[0].CompareTo(b[0]));

                if (DIPSMatches < 3 | (DIPSMatches == 3 && EFSExists))// if dips frontmatter and endmatter are found but no folios, then dipsmatches = 2 - BAD
                                                                     // if front matter, folios + endmatter found (might not be any flysheets) dipsmatches = 3 FINE
                                                                     // if DIPSMatches ==3 and end flysheets exist, one of folios, frontmatter or endmatter isn't dips compliant and we should flag 
                {
                    List<string> errorFlag = new List<string> { "Mixture of DIPS-compliant and non-compliant filenames in this shelfmark!" };
                    //allFilesSorted.flagStatus.Add(errorFlag);
                    allFilesSorted.Add(errorFlag);
                }

                foreach (List<String> fmList in frontMatter)
                {
                    allFilesSorted.Add(fmList);
                }
                foreach (List<String> fmList in folios)
                {
                    allFilesSorted.Add(fmList);
                }
                foreach (List<String> fmList in endFlysheets)
                {
                    allFilesSorted.Add(fmList);
                }
                foreach (List<String> fmList in endMatter)
                {
                    allFilesSorted.Add(fmList);
                }


            } // if DIPs compliant
            else // is fully non-DIPS compliant and just has numerical filenames, so just sort this normally
            {
               List<String> sortedFilenames = fileNames.OrderBy(x => x).Select(x => x.ToString()).ToList();
                foreach (var sfn in sortedFilenames)
                {
                    List<String> nums = new List<String>();
                    nums.Add(sfn);
                    nums.Add(""); // errorString
                    nums.Add("page");
                    string[] split = sfn.Split('.');
                    string shelfmark_filename = string.Join(".", split.Take(split.Length - 1)); // shelfmark_filename
                    string fileExtension = split.Last(); // tif
                    string[] split2 = shelfmark_filename.Split('_');
                    string derivedShelfmark = string.Join(".", split2.Take(split2.Length - 1)); // shelfmark
                    string derivedFilename = split2.Last();
                    string noZerosName = derivedFilename.TrimStart('0');
                    noZerosName = noZerosName.Length > 0 ? noZerosName : "0";
                    nums.Add(noZerosName); // just get the number from the filename

                    allFilesSorted.Add(nums);
                }
            }

            // At this stage you have allFilesSorted as a list-of-lists with
           // filename , flagStatus, objectType, Label 
           //- flagStatus is a string that is either empty (all good!) or contains an error message
           // objectType is jut page, cover, flysheet etc
           // label is "back cover inside", "folio 5v" etc
           for(int i = 0; i<allFilesSorted.Count; i++)
            {
                string orderNumber = (i + 1).ToString();
                allFilesSorted[i].Add(orderNumber);
                

            }

           // now allFilesSorted contains 
           //[0]: filename
           //[1]: flagStatus
           //[2]: objectType
           //[3]: label
           //[4]: order number

            // Should just create a file label class... to-do!

            // Still need to check if image file names have consecutive numbering

            // Option for just doing the regex on the list instead:
            /*
            List<string> frontMatter = fileNames.Where(f => frontMatterRegex.IsMatch(f)).ToList();
            List<string> endFlysheets = fileNames.Where(f => endFlysheetsRegex.IsMatch(f)).ToList();
            List<string> endMatter = fileNames.Where(f => endMatterRegex.IsMatch(f)).ToList();
            List<string> folios = fileNames.Where(f => folioRegex.IsMatch(f)).ToList();

            List<String> allFilesSorted = new List<String>();
            if (frontMatter.Any() | folios.Any()) // is DIPS format
            {
                List<String> sortedFrontMatter = frontMatter.OrderBy(x => x).ToList();
                List<String> sortedEndMatter = endMatter.OrderBy(x => x).ToList();
                List<String> sortedEndFlysheets = endFlysheets.OrderBy(x => x).ToList();
                List<String> sortedFolios = folios.OrderBy(x => x).ToList();
                allFilesSorted = sortedFrontMatter.Concat(sortedFolios).Concat(sortedEndFlysheets).Concat(sortedEndMatter).ToList();
            }
            else
            {
                allFilesSorted = fileNames.OrderBy(x => x).ToList();
            }
            int counter = 1;
            */

            return allFilesSorted;
        }


        public static bool RetrieveImgOrderLabels(List<List<String>> allShelfmarkFiles, String env)
        {
            bool fError = false;

            if (env == "test")
            {
                try
                {
                    foreach (List<String> shelfmarkFiles in allShelfmarkFiles)
                    {
                        // ======================== this block is just used for testing locally =====================

                        string filePath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                        filePath += @"\HMDSharepoint_ImgOrderTest\";
                        string SM_folderFormat = shelfmarkFiles[0].ToLower().Replace(@" ", @"_").Replace(@"/", @"!").Replace(@".", @"_").Replace(@"*", @"~");
                        filePath += SM_folderFormat;

                        if (!Directory.Exists(filePath))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        // ===============================================================
                        foreach (var thing in shelfmarkFiles)
                        {
                            // Create a directory on the user's desktop if it doesn't already exist

                            Console.WriteLine("{0}", thing);
                        }
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("ERROR: {0}", ex);
                    fError = true;
                    return !fError;
                }
                   
            }
            else if(env == "prod")
            {
                //do whatever you'd want to do in prod - we're only at test for now
            }
            else // env is ill-defined
            {
                fError = true;
            }


            return !fError;
        }

        private static bool writeFileLabelsToCSV(List<List<String>> allShelfmarkFiles)
        {
            bool fError = false;

            try // to write the csv...
            {
                const char sep = ',';
                List<String> strHeaders = new List<string>{"File","Order","Type","Label"};
                System.Text.UnicodeEncoding uce = new System.Text.UnicodeEncoding();
                using (var sr = new StreamWriter("test.txt", false, uce))
                {
                    using (var csvFile = new CsvHelper.CsvWriter(sr, System.Globalization.CultureInfo.InvariantCulture))
                    {
                        foreach (var header in strHeaders)
                        {
                            csvFile.WriteField(header);
                        }
                        csvFile.NextRecord(); // should skip over header?
                        foreach (var record in allShelfmarkFiles)
                        {
                            for (int i = 0; i < record.Count; i++)
                            {
                                // now allFilesSorted contains 
                                //[0]: filename
                                //[1]: flagStatus
                                //[2]: objectType
                                //[3]: label
                                //[4]: order number
                                csvFile.WriteField(record[0]); // filename
                                csvFile.WriteField(record[4]); // order number
                                csvFile.WriteField(record[2]); // object type
                                csvFile.WriteField(record[3]); // label
                                csvFile.WriteField(record[1]); // error flag status
                            }
                            if (allShelfmarkFiles.IndexOf(record) != allShelfmarkFiles.Count - 1)
                            {
                                csvFile.NextRecord();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing CSV File: {0}", ex);
                fError = true;
            }
            return !fError;
        }
    }
}

