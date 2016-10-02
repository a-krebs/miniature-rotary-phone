# Miniature-Rotary-Phone
## Miscommunication Game 
For Game Start exhibition at the Latitude 53 gallery

## Network Config Files
At game start, there must be a `network_config.json` file in the current
working directory. That file must contain:

| Config Parameter | Value |
| ------------- | ------------- |
| `role` | One of `client`, `server`, or `host` (client that also runs as server) |
| `server-address` | IPv4 address of the host or server. |
| `server-port` | Post on which the server/host is to listen, and on which the client is to connect. |

### Example network_config.json
```
{
	"role": "host",
	"server-address": "192.168.1.123",
	"server-port": "7777"
}
```
