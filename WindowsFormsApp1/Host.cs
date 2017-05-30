using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    class Host
    {
        private String username;
        private Image picture;
        private String ip_addr;
        private Boolean online;
        private DateTime last_alive;

        public Host(String username, Image picture, String ip)
        {
            this.username = username;
            this.picture = picture;
            this.ip_addr = ip;
        }

        public void setOnline()
        {
            online = true;
        }

        public void setOffline()
        {
            online = false;
        }

        public Boolean isOnline()
        {
            return online;
        }

        public DateTime lastAlive()
        {
            return last_alive;
        }

        public void alive()
        {
            last_alive = DateTime.Now;
        }

    }
}
