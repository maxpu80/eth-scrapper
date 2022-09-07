namespace Infra.AzureBlob

open System.IO
open System.Text

[<RequireQualifiedAccess>]
module BlobOperators =
  open Azure.Storage.Blobs
  let readAsStream (blobClient: BlobClient) = blobClient.OpenReadAsync()

  let writeStream (blobContainerClient: BlobContainerClient) (blobName: string) (stream: Stream) =
    blobContainerClient.UploadBlobAsync(blobName, stream)

  let writeString (blobContainerClient: BlobContainerClient) (blobName: string) (data: string) =
    use stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(data))
    writeStream blobContainerClient blobName stream
