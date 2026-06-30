---@meta
---Stub Types For FluxCon API

---@class Flux
Flux = {}

---@enum LoggerState
Flux.LoggerState = {
	NotStarted = 0,
	Connecting = 1,
	Connected = 2,
	Reconnecting = 3,
	ShuttingDown = 4,
}

---@enum ModType
Flux.ModType = {
	None = 0,
	Lua = 1,
	Cpp = 2,
	Blueprint = 4,
	Pak = 8,

	LuaCpp = 3,
	LuaBlueprint = 5,
	CppBlueprint = 6,
	LuaCppBlueprint = 7,
}

---@enum LogLevel
Flux.LogLevel = {
	Info = 0,
	Debug = 1,
	Verbose = 2,
	Warning = 3,
	Error = 4,
	Fatal = 5,
}

---@enum Exception
Flux.Exception = {
	None = 0,
	UnknownRuntimeError = 1,
	Timeout = 2,
	InvalidCast = 3,
	KeyNotFound = 4,
	PathNotFound = 5,
	NullReference = 6,
	InvalidArgument = 7,
	IndexOutOfBounds = 8,
	InvalidOperation = 9,
	ApiVersionMismatch = 10,
}

---@class ModInfo
---@field name string Folder Name Of Your Mod
---@field displayName string Display Name Of Your Mod
---@field author string Mod Author Name
---@field version string Semantic Version String (eg. "1.0.0")
---@field type ModType|integer Type Of Your Mod
---@field nexusLink string|nil Nexus Mods Link
---@field gitHubLink string|nil GitHub Link
---@field dependencies string[] List Of UNIQUE Folder Names Of Your Mod Dependencies

---Gets Current Logger State
---@return LoggerState
function Flux.GetLoggerState() end

---Registers The Mod To FluxCon (Required For Features)
---@param info ModInfo
function Flux.RegisterMod(info) end

---UnRegisters The Current Mod From FluxCon
function Flux.UnRegisterMod() end

---@param message string The Message To Log
---@param ... any Optional Args
---@overload fun(exception: Exception, message: string, ...: any)
function Flux.Info(message, ...) end

---@param message string The Message To Log
---@param ... any Optional Args
---@overload fun(exception: Exception, message: string, ...: any)
function Flux.Debug(message, ...) end

---@param message string The Message To Log
---@param ... any Optional Args
---@overload fun(exception: Exception, message: string, ...: any)
function Flux.Verbose(message, ...) end

---@param message string The Message To Log
---@param ... any Optional Args
---@overload fun(exception: Exception, message: string, ...: any)
function Flux.Warn(message, ...) end

---@param message string The Message To Log
---@param ... any Optional Args
---@overload fun(exception: Exception, message: string, ...: any)
function Flux.Error(message, ...) end

---@param message string The Message To Log
---@param ... any Optional Args
---@overload fun(exception: Exception, message: string, ...: any)
function Flux.Fatal(message, ...) end

