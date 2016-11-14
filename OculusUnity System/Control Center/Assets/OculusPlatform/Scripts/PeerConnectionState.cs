// This file was @generated with LibOVRPlatform/codegen/main. Do not modify it!

namespace Oculus.Platform
{

  using Description = System.ComponentModel.DescriptionAttribute;

  public enum PeerConnectionState : uint
  {
    [Description("UNKNOWN")]
    Unknown,

    /// Connection to the peer has been established.
    [Description("CONNECTED")]
    Connected,

    /// A timeout expired while attempting to (re)establish a connection.  This
    /// can happen if peer is unreachable or rejected the connection.
    [Description("TIMEOUT")]
    Timeout,

    /// Connection to the peer has been closed.  A connection transitions into
    /// this state when it's explicitly closed by either the local or remote peer
    /// calling ovr_Net_Close(), but also if the remote peer no longer responds to
    /// our keep-alive probes.
    [Description("CLOSED")]
    Closed,

  }

}
