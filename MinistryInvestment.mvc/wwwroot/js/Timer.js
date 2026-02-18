class Timer {
    constructor(warningTime, timeoutTime, keepSessionAliveURL, getLastSessionTimeUrl) {
        this.warningTime = warningTime * 60;
        this.timeoutTime = timeoutTime * 60;
        this.elapsedTime = 0; // seconds
        this.currentTime = timeoutTime;
        this.interval = null;
        this.sessionTime = null;
        this.keepSessionAliveURL = keepSessionAliveURL;
        this.getLastSessionTimeUrl = getLastSessionTimeUrl;
    }

    start() {
        this.interval = setInterval(async () => {
            this.currentTime -= 1;
            this.elapsedTime += 1;
            let previousSessionTime = this.sessionTime;
            let minutes = Math.floor(this.currentTime / 60);
            let seconds = this.currentTime % 60;

            $('#time').text(minutes + ":" + (seconds > 10 ? seconds : "0" + seconds));
            if (this.elapsedTime === this.warningTime) { // 15 minutes have elapsed, in reality
                await this.checkSession(previousSessionTime);
            }

            if (this.elapsedTime >= this.timeoutTime) {
                await this.checkSession(previousSessionTime);
            }

        }, 1000);
    }

    stop() {
        clearTimeout(this.interval);
        this.interval = null;
    }

    async checkSession(previousSessionTime) {
        const diff = Math.floor((new Date().getTime() - new Date(await this.getSessionTime()).getTime()) / 1000);
        if (previousSessionTime === await this.getSessionTime()) {
            $('#timeout').modal('show');
            if (diff >= this.timeoutTime) {
                $("#formLogout").submit();
            }
        } else {
            timer.reset(diff);
            $('#timeout').modal('hide');
        }
    }

    async reset(diff) {
        this.stop();
        this.elapsedTime = diff ? diff : 0;
        this.currentTime = this.timeoutTime;
        this.sessionTime = await this.getSessionTime();
        this.start();
    }

    async getSessionTime() {
        const response = await fetch(this.getLastSessionTimeUrl, { method: "GET" });
        const json = await response.json();
        return json;
    }

    async keepSessionAlive() {
        await fetch(this.keepSessionAliveURL, { method: "GET" });
        this.reset();
    }
}

