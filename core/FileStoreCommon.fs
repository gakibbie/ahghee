namespace Ahghee

open Google.Protobuf
open Google.Protobuf.Collections
open System
open System.Collections.Generic
open System.Diagnostics
open System.Drawing
open System.IO
open System.Threading
open System.Threading.Tasks
open Ahghee.Grpc

type Config = {
    ParitionCount:int
    log: string -> unit
    CreateTestingDataDirectory:bool
    }


type NodeIO =
    | Write of TaskCompletionSource<unit> * seq<Node>
    | Read  of TaskCompletionSource<Node> * MemoryPointer
    | FlushFixPointers of TaskCompletionSource<unit>
    | FlushWrites of TaskCompletionSource<unit>
    | FlushFragmentLinks of TaskCompletionSource<unit>

type IndexMessage =
    | Index of Grpc.NodeID
    | Flush of AsyncReplyChannel<bool>

[<System.FlagsAttribute>]
type WriteGroupsOperation = LinkFragments = 1 | FixPointers = 2  
    
type NodeIOGroup = { start:uint64; length:uint64; items:List<MemoryPointer> }

type IClusterServices =
    abstract RemoteLookup : int -> NodeIdHash -> bool * MemoryPointer
