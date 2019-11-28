var router = require('express').Router();

router.post('/', async function(req, resp, next) {
    if (req.isAuthorized) {
        resp.redirect('/');
    } else {
        
        let res = await req.app.get('orm').tables.users.filter({login: req.body.login});
        if (res.length > 0)
            resp.sendStatus(409).end();
        else {
            let id = await req.app.get('orm')
                    .tables.users.add({
                        login: req.body.login, 
                        password: req.body.password, 
                        email: req.body.email,
                        firstName: req.body.fname,
                        secondName: req.body.sname
                    });
            if (Number.isInteger(id))
                resp.cookie('auth', await req.authenticator.add(id));
            resp.redirect('/');
        }        
    }
});

module.exports = router;