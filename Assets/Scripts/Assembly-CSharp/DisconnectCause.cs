using System;

public enum DisconnectCause
{
	AuthenticationTicketExpired = 32753,
	DisconnectByClientTimeout = 1040,
	[Obsolete("Replaced by clearer: DisconnectByServerTimeout")]
	DisconnectByServer = 1041,
	DisconnectByServerLogic = 1043,
	DisconnectByServerTimeout = 1041,
	DisconnectByServerUserLimit = 1042,
	Exception = 1026,
	ExceptionOnConnect = 1023,
	InternalReceiveException = 1039,
	InvalidAuthentication = 32767,
	InvalidRegion = 32756,
	MaxCcuReached = 32757,
	SecurityExceptionOnConnect = 1022,
	[Obsolete("Replaced by clearer: DisconnectByClientTimeout")]
	TimeoutDisconnect = 1040
}
