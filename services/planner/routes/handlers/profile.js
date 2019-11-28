var router = require('express').Router();

router.get('/', async function(req, resp, next) {
    const res = await req.app.get('orm')
                        .tables.users.filter({
                            id: req.authedUserId
                        });
    
    if (res.length == 0)
        resp.sendStatus(404);
    const user = res[0];
    
    resp.render('profile.html', {isAuthed: true, user: user});
});

router.post('/', async function(req, resp, next) {
    const res = await req.app.get('orm')
                        .tables.users.filter({
                            id: req.authedUserId
                        });
    
    if (res.length == 0)
        resp.sendStatus(404);
    const user = res[0];
    
    let user_check_res = await req.app.get('orm').tables.users.filter({login: req.body.login});

    if (user_check_res.length > 0 && user.login != user_check_res[0].login)
        resp.sendStatus(409).end();
    else {
        let updated_user = {};
        Object.keys(user).forEach(function(field){if (req.body[field] && req.body[field]!=user[field]) updated_user[field] = req.body[field]});
        if (Object.keys(updated_user).length !== 0){
            const update_res = await req.app.get('orm')
                                .tables.users.update( {id: user.id}, updated_user); console.log(update_res);}
        resp.json("SRABOTALO");
    }
});

module.exports = router;