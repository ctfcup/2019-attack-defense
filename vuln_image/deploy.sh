#!/bin/bash

set -e

# Cd into script directory
BASE_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null && pwd )"
cd "$BASE_DIR"

if [ "$#" -ne 1 ]; then
    echo "Usage: $0 service_name"
    exit 2
fi

SERVICE=$1

echo "Checking card mount"
if (mount | grep mmcblk); then
    echo "Card is mounted, unmounting before image write"
    umount /dev/mmcblk0p1
else
    echo "Card is unmounted. Proceeding"
fi

echo "Writing using dd"
time dd if=images/base.img of=/dev/mmcblk0 bs=1M status=progress
echo "Syncing"
time sync
echo "Done"

echo "Mounting to modify"
mkdir -p card
mount /dev/mmcblk0p1 card

echo "Setting dhclient user-class"
sed -i "/^send host-name.*/a send user-class \"$SERVICE\";" card/etc/dhcp/dhclient.conf
#echo "$SERVICE" > card/etc/hostname

echo "Setting up ssh key"
mkdir -p card/root/.ssh/
cat keys/id_rsa.pub > card/root/.ssh/authorized_keys

echo "Resetting password"
sed -i 's/^root:.*/root:!:18166:0:99999:7:::/' card/etc/shadow

# this doesn't work =(
#echo -e "suchsecret\nsuchsecret" | passwd -R ./card/ root

echo "Unmounting card"
sync
umount ./card

echo "Everything done, time to test!"

