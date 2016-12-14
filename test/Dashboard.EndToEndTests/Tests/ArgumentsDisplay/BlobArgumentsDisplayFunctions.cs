// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;

namespace Dashboard.EndToEndTests
{
    internal class BlobArgumentsDisplayFunctions
    {
        internal class POCOBinder : ICloudBlobStreamBinder<POCO>
        {
            public Task<POCO> ReadFromStreamAsync(Stream input, CancellationToken cancellationToken)
            {
                using (StreamReader reader = new StreamReader(input))
                {
                    POCO result = JsonConvert.DeserializeObject<POCO>(reader.ReadToEnd());
                    return Task.FromResult(result);
                }
            }

            public Task WriteToStreamAsync(POCO value, Stream output, CancellationToken cancellationToken)
            {
                StreamWriter writer = new StreamWriter(output);

                string result = JsonConvert.SerializeObject(value);
                writer.Write(result);
                writer.Flush();

                return Task.FromResult(0);
            }
        }

        public const string ContainerName = "blob-arguments";

        public static readonly MethodInfo StreamMethodInfo = typeof(BlobArgumentsDisplayFunctions).GetMethod("Stream");
        public static readonly MethodInfo TextReaderWriterMethodInfo = typeof(BlobArgumentsDisplayFunctions).GetMethod("TextReaderWriter");
        public static readonly MethodInfo StringMethodInfo = typeof(BlobArgumentsDisplayFunctions).GetMethod("String");
        public static readonly MethodInfo CloudBlockBlobMethodInfo = typeof(BlobArgumentsDisplayFunctions).GetMethod("CloudBlockBlob");
        public static readonly MethodInfo POCOMethodInfo = typeof(BlobArgumentsDisplayFunctions).GetMethod("POCO");

        public static async Task Stream(
            [BlobTrigger(ContainerName + "/stream-trigger")] Stream trigger,
            [Blob(ContainerName + "/stream-in", FileAccess.Read)] Stream input,
            [Blob(ContainerName + "/stream-out", FileAccess.Write)] Stream output,
            [Queue(DoneNotificationFunction.DoneQueueName)] IAsyncCollector<string> done)
        {
            await trigger.CopyToAsync(output);
            await input.CopyToAsync(output);
            await done.AddAsync("x");
        }

        public static void TextReaderWriter(
           [BlobTrigger(ContainerName + "/textreaderwriter-trigger")] TextReader trigger,
           [Blob(ContainerName + "/textreaderwriter-in")] TextReader input,
           [Blob(ContainerName + "/textreaderwriter-out")] TextWriter output,
           [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output.Write(trigger.ReadToEnd());
            output.Write(input.ReadToEnd());
            done = "x";
        }

        public static void String(
           [BlobTrigger(ContainerName + "/string-trigger")] string trigger,
           [Blob(ContainerName + "/string-in")] string input,
           [Blob(ContainerName + "/string-out")] out string output,
           [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = trigger + input;
            done = "x";
        }

        public static void CloudBlockBlob(
           [BlobTrigger(ContainerName + "/cloudblockblob-trigger")] CloudBlockBlob trigger,
           [Blob(ContainerName + "/cloudblockblob-in")] CloudBlockBlob input,
           [Blob(ContainerName + "/cloudblockblob-out")] CloudBlockBlob output,
           [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            string content = trigger.DownloadText() + input.DownloadText();
            output.UploadText(content);
            done = "x";
        }

        public static void POCO(
           [BlobTrigger(ContainerName + "/poco-trigger")] POCO trigger,
           [Blob(ContainerName + "/poco-in")] POCO input,
           [Blob(ContainerName + "/poco-out")] out POCO output,
           [Queue(DoneNotificationFunction.DoneQueueName)] out string done)
        {
            output = new POCO()
            {
                StringValue = trigger.StringValue + input.StringValue,
                IntValue = trigger.IntValue + input.IntValue
            };
            done = "x";
        }
    }
}
