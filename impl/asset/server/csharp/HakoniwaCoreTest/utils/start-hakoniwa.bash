#/bin/bash

if [ $# -ne 2 -a $# -ne 3 ]
then
	echo "Usage: $0 <proj> <eth> [simtime_filepath]"
	exit 1
fi

HAKONIWA_PRJ=${1}
ETH=${2}
HAKONIWA_CFG_TMPL=utils/config_proxy_udp_json.mo
SYMTIME_MEASURE_FILEPATH=".\\core.csv"
if [ $# -eq 3 ]
then
	TMPDIR=$(cd ${3} && pwd)
	DIR=`echo "${TMPDIR}/core.csv" | sed 's/\/mnt\/c\//C:\\\\\\\\/g' | sed 's/\//\\\\\\\\/g'`
	export SYMTIME_MEASURE_FILEPATH=${DIR}
fi

if [ -d ${HAKONIWA_PRJ} ]
then
	:
else
	echo "ERROR: not found ${HAKONIWA_PRJ}"
	exit 1
fi

export IFCONFIG_IPADDR=`ifconfig | grep -A 1 ${ETH} | grep inet | awk '{print $2}'`
export RESOLVE_IPADDR=`cat /etc/resolv.conf | grep nameserver | awk '{print $2}'`


bash utils/mo ${HAKONIWA_CFG_TMPL} > ${HAKONIWA_PRJ}/core_config.json
cd ./${HAKONIWA_PRJ}
./HakoniwaCoreTest.exe
