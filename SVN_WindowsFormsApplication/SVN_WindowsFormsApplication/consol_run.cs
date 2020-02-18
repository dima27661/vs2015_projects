using System;
using System.IO;
using System.Net;
using SharpSvn;
using System.Collections.ObjectModel;


namespace SvnClient
{
    class Program
    {
      public static void Main_consol(string[] args)
        {
            // This doesn't affect SVN server certs
            //ServicePointManager.ServerCertificateValidationCallback += delegate { Console.WriteLine("ServerCertificateValidationCallback!"); return true; };

            Parameters parameters;
            if (!Parameters.TryParse(args, out parameters))
            {   
                return;
            }
            //костыль
            //if (parameters.Path.IndexOf("--message=") > 0)

            //{
            //    parameters.Path = args[2];
            //    parameters.Path = args[2];   }

            switch (parameters.Command)
            {
                case Command.CompleteSync:
                    CompleteSync(parameters);
                    break;
                case Command.CheckoutUpdate:
                    CheckoutUpdate(parameters);
                    break;
            }
        }

        public static string GetLogText(string[] args, string revision,  out Collection<SvnLogEventArgs> logItemsColl)
        {
            Parameters parameters;
            if (!Parameters.TryParse(args, out parameters))
            {
                throw new Exception("TryParse failed");
            }

            using (var client = new SharpSvn.SvnClient())
            {
                SetUpClient(parameters, client);

                var target = SvnTarget.FromString(parameters.Path);
                SvnInfoEventArgs svnInfoEventArgs;
                SvnUpdateResult svnUpdateResult;

                if (!client.GetInfo(target, out svnInfoEventArgs))
                {
                    throw new Exception("SVN info failed");
                }

                 Uri svnUrl =  svnInfoEventArgs.Uri ;
                long revisionLong = long.Parse(revision);

                long revisionLongTo =  svnInfoEventArgs.Revision;


                SvnLogArgs Logargs = new SvnLogArgs
                {
                    Range = new SvnRevisionRange(revisionLong, revisionLongTo)
                };
                Collection<SvnLogEventArgs> logItems;
               //  client.GetLog(svnUrl, Logargs, out logItems);
                client.GetLog(svnUrl,  out logItemsColl);

                string AlllogText="";
                for (int i = logItemsColl.Count-1 ; i > 0; i--)
                {
                    AlllogText = AlllogText + (Char)13 + (Char)10 +
                      logItemsColl[i].Revision.ToString() + " " + logItemsColl[i].Time.ToString() + " " + logItemsColl[i].LogMessage   ;
                }



                return AlllogText;

                throw new Exception("SVN update failed");
            }







            return "jhgjg";

        }
        public static string GetInfo(string[] args, string revision)
        {
            Parameters parameters;
            if (!Parameters.TryParse(args, out parameters))
            {
                return "False";
            }

            using (var client = new SharpSvn.SvnClient())
            {
                SetUpClient(parameters, client);

                var target = SvnTarget.FromString(parameters.Path);
                SvnInfoEventArgs svnInfoEventArgs;
                SvnUpdateResult svnUpdateResult;

                if (!client.GetInfo(target, out svnInfoEventArgs))
                {
                    throw new Exception("SVN info failed");
                }
                Uri svnUrl = svnInfoEventArgs.Uri;
                return svnInfoEventArgs.Revision.ToString(); 

                throw new Exception("SVN update failed");
            }
            return "jhgjg";
        }
        public static string UpdateToRevision(string[] args, string revision)
        {
            Parameters parameters;
            if (!Parameters.TryParse(args, out parameters))
            {
                return "False";
            }

            using (var client = new SharpSvn.SvnClient())
            {
                SetUpClient(parameters, client);

                var target = SvnTarget.FromString(parameters.Path);
                SvnInfoEventArgs svnInfoEventArgs;
                SvnUpdateResult svnUpdateResult;

                if (!client.GetInfo(target, out svnInfoEventArgs))
                {
                    throw new Exception("SVN info failed");
                }

                Uri svnUrl = svnInfoEventArgs.Uri;
                long revisionLong = long.Parse(revision);


                //update to revision

                //DebugMessage(parameters, "Updating");
                SvnUpdateArgs SvnArgs = new SvnUpdateArgs();
                //// If args.Revision is not set, it defaults to fetch the HEAD revision.
                    SvnArgs.Revision = revisionLong;
                if (client.Update(parameters.Path, SvnArgs, out svnUpdateResult))
                {
                    DebugMessage(parameters, "Done");

                    Console.WriteLine("Updated to r" + svnUpdateResult.Revision);
                    return "true";
                }
                throw new Exception("SVN update failed");
            }
            return "jhgjg";
        }


        private static void CheckoutUpdate(Parameters parameters)
        {
            using (var client = new SharpSvn.SvnClient())
            {
                SetUpClient(parameters, client);

                var target = SvnTarget.FromString(parameters.Path);
                SvnInfoEventArgs svnInfoEventArgs;
                SvnUpdateResult svnUpdateResult;

                var nonExistentUrl = false;
                EventHandler<SvnErrorEventArgs> ignoreNonexistent = (o, eventArgs) =>
                                                                        {
                                                                            nonExistentUrl = false;
                                                                            //if (eventArgs.Exception.SubversionErrorCode == 170000)
                                                                            if(eventArgs.Exception.Message.Contains("non-existent in revision"))
                                                                            {
                                                                                nonExistentUrl = true;
                                                                                eventArgs.Cancel = true;
                                                                            }
                                                                        };

                if(client.GetWorkingCopyRoot(parameters.Path) == null)
                {
                    client.SvnError += ignoreNonexistent;
                    var getInfoSucceeded = client.GetInfo(SvnUriTarget.FromString(parameters.Url), out svnInfoEventArgs);
                    client.SvnError -= ignoreNonexistent;

                    if(!getInfoSucceeded)
                    {
                        if (nonExistentUrl)
                        {
                            Console.WriteLine("SVN info reported nonexistent URL; creating remote directory.");
                            if (!client.RemoteCreateDirectory(new Uri(parameters.Url), new SvnCreateDirectoryArgs {CreateParents = true, LogMessage = parameters.Message}))
                            {
                                throw new Exception("Create directory failed on " + parameters.Url);
                            }
                        }
                        else
                        {
                            throw new Exception("SVN info failed");
                        }
                    }

                    DebugMessage(parameters, "Checking out");
                    if (client.CheckOut(SvnUriTarget.FromString(parameters.Url), parameters.Path, out svnUpdateResult))
                    {
                        DebugMessage(parameters, "Done");
                        Console.WriteLine("Checked out r" + svnUpdateResult.Revision);
                        return;
                    }

                    throw new Exception("SVN checkout failed");
                }

                if(!client.GetInfo(target, out svnInfoEventArgs))
                {
                    throw new Exception("SVN info failed");
                }

                if(!UrlsMatch(svnInfoEventArgs.Uri.ToString(), parameters.Url))
                {
                    throw new Exception(string.Format("A different URL is already checked out ({0} != {1})", svnInfoEventArgs.Uri, parameters.Url));
                }
                
                if(parameters.Cleanup)
                {
                    DebugMessage(parameters, "Cleaning up");
                    client.CleanUp(parameters.Path);
                    DebugMessage(parameters, "Done");
                }

                if(parameters.Revert)
                {
                    DebugMessage(parameters, "Reverting");
                    client.Revert(parameters.Path);
                    DebugMessage(parameters, "Done");
                }

                if (parameters.DeleteUnversioned)
                {
                    DebugMessage(parameters, "Deleting unversioned files");
                    Collection<SvnStatusEventArgs> changedFiles;
                    client.GetStatus(parameters.Path, out changedFiles);
                    foreach (var changedFile in changedFiles)
                    {
                        if(changedFile.LocalContentStatus == SvnStatus.NotVersioned)
                        {
                            if(changedFile.NodeKind == SvnNodeKind.Directory)
                            {
                                DebugMessage(parameters, "NodeKind is directory for [" + changedFile.FullPath + "]");
                            }
                            if ((File.GetAttributes(changedFile.FullPath) & FileAttributes.Directory) == FileAttributes.Directory)
                            {
                                DebugMessage(parameters, "Deleting directory [" + changedFile.FullPath + "] recursively!");
                                Directory.Delete(changedFile.FullPath, true);
                            }
                            else
                            {
                                DebugMessage(parameters, "Deleting file [" + changedFile.FullPath + "]");
                                File.Delete(changedFile.FullPath);
                            }
                        }
                    }
                    DebugMessage(parameters, "Done");
                }

                DebugMessage(parameters, "Updating");
                if(client.Update(parameters.Path, out svnUpdateResult))
                {
                    DebugMessage(parameters, "Done");
                    Console.WriteLine("Updated to r" + svnUpdateResult.Revision);
                    return;
                }

                throw new Exception("SVN update failed");
            }
        }

        private static bool UrlsMatch(string url1, string url2)
        {
            if (url1 == url2) return true;

            // Ignore trailing slash
            return url1.TrimEnd('/') == url2.TrimEnd('/');
        }

        // TODO: make trusting not the default
        private static void TrustUnsignedCertificates(SharpSvn.SvnClient client)
        {
            client.Authentication.SslServerTrustHandlers += (sender, e) =>
                                                                {
                                                                    Console.WriteLine("Certificate errors: " + e.Failures);
                                                                    e.AcceptedFailures = e.Failures;
                                                                    e.Save = true; // Save it for future use?

                                                                };
            //client.Authentication.SslServerTrustHandlers += Authentication_SslServerTrustHandlers;
        }

        public static void CompleteSync(Parameters parameters)
        {
            using (var client = new SharpSvn.SvnClient())
            {
                SetUpClient(parameters, client);

                if (parameters.UpdateBeforeCompleteSync)
                {
                    DebugMessage(parameters, "Updating");
                    client.Update(parameters.Path);
                    DebugMessage(parameters, "Done");
                }

                Collection<SvnStatusEventArgs> changedFiles;
                DebugMessage(parameters, "Getting status");
                client.GetStatus(parameters.Path, out changedFiles);
                DebugMessage(parameters, "Done");
                if(changedFiles.Count == 0)
                {
                    Console.WriteLine("No changes to commit.");
                    return;
                }

                //delete files from subversion that are not in filesystem
                //add files to subversion that are new in filesystem
                //modified files are automatically included as part of the commit

                //TODO: check remoteStatus
                DebugMessage(parameters, "Recording changes");
                foreach (var changedFile in changedFiles)
                {
                    if (changedFile.LocalContentStatus == SvnStatus.Missing)
                    {
                        // SVN thinks file is missing but it still exists hence
                        // a change in the case of the filename.
                        if (File.Exists(changedFile.Path))
                        {
                            var changedArgs = new SvnDeleteArgs {KeepLocal = true};
                            client.Delete(changedFile.Path, changedArgs);
                        }
                        else
                            client.Delete(changedFile.Path);
                    }
                    if (changedFile.LocalContentStatus == SvnStatus.NotVersioned)
                    {
                        client.Add(changedFile.Path);
                    }
                }
                DebugMessage(parameters, "Done");

                var ca = new SvnCommitArgs {LogMessage = parameters.Message};
                SvnCommitResult result;
                DebugMessage(parameters, "Committing");
                if (client.Commit(parameters.Path, ca, out result))
                {
                    DebugMessage(parameters, "Done");
                    if(result == null)
                    {
                        Console.WriteLine("No result returned from commit.");
                        return;
                    }
                    if (!string.IsNullOrEmpty(result.PostCommitError))
                    {
                        Console.WriteLine("Post-commit hook error: " + result.PostCommitError);
                        return;
                    }
                    Console.WriteLine("Committed r" + result.Revision);
                }
                else
                {
                    if (result != null && !string.IsNullOrEmpty(result.PostCommitError))
                    {
                        Console.WriteLine("Post-commit hook error after failed commit: " + result.PostCommitError);
                        return;
                    }
                    Console.WriteLine("Commit failed.");
                }
            }
        }

        private static void SetUpClient(Parameters parameters, SharpSvn.SvnClient client)
        {
            if (!string.IsNullOrEmpty(parameters.Username) && !string.IsNullOrEmpty(parameters.Password))
            {
                client.Authentication.Clear(); // prevents checking cached credentials
                client.Authentication.DefaultCredentials = new NetworkCredential(parameters.Username, parameters.Password);
            }

            if(parameters.TrustServerCert)
            {
                TrustUnsignedCertificates(client);
            }
        }

        private static void DebugMessage(Parameters parameters, string format, params object[] args)
        {
            if(parameters.Verbose)
            {
                Console.Write(DateTime.Now + "\t");
                Console.WriteLine(string.Format(format, args));
            }
        }
        public static void Diff_ReposDiff(string[] args, long Rev1, long Rev2 )
        {
            //string clientDiff = this.RunCommand("svn", "diff -r 1:5 " + this.ReposUrl);

            MemoryStream outstream = new MemoryStream();
            MemoryStream errstream = new MemoryStream();

            SvnDiffArgs a = new SvnDiffArgs();

            Parameters parameters;
            if (!Parameters.TryParse(args, out parameters))
            {
                return;
            }

            using (var client = new SharpSvn.SvnClient())
            {
                SetUpClient(parameters, client);

                var target = SvnTarget.FromString(parameters.Path);
                SvnInfoEventArgs svnInfoEventArgs;
                SvnUpdateResult svnUpdateResult;

                if (!client.GetInfo(target, out svnInfoEventArgs))
                {
                    throw new Exception("SVN info failed");
                }

                Uri svnUrl = svnInfoEventArgs.Uri;

                a.ErrorStream = errstream;
                
                try
                {
                 client.Diff(target, new SvnRevisionRange(Rev1, Rev2), a, outstream);
                }

                catch (Exception)
                {
                    throw new Exception("SVN diff failed");
                 //  return ;
                }


                return ;

               
            }




            //string err = Encoding.Default.GetString(errstream.ToArray());
            //Assert.That(err, Is.EqualTo(string.Empty), "Error in diff: " + err);

            //string apiDiff = Encoding.Default.GetString(outstream.ToArray());
            //string[] clientLines = clientDiff.Split('\n');
            //string[] apiLines = apiDiff.Split('\n');
            //Array.Sort<string>(clientLines);
            //Array.Sort<string>(apiLines);

            //Assert.That(apiLines, Is.EqualTo(clientLines), "Diffs differ");
        }




    }
}
