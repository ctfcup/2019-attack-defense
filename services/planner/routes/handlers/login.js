var router = require('express').Router();

router.post('/', async function(req, resp, next) {
    if (req.isAuthorized) {
        resp.redirect('/');
    } else {
        let res = await req.app.get('orm')
                            .tables.users.filter({
                                login: req.body.login, 
                                password: req.body.password
                            });
        if (res.length == 1) {
            resp.cookie('auth', await req.authenticator.add(res[0].id));
            resp.sendStatus(200);
        } else
            resp.sendStatus(401);
    }
});

module.exports = router;