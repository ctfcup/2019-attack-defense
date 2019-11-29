const auth = require('./authenticator');
var router = require('express').Router();


router.get('/', auth.optional, async function(req, resp, next) {
    resp.render('index.html', {isAuthed: req.isAuthorized});
});

router.use('/registration', auth.optional, require('./handlers/registration'));
router.use('/login', auth.optional, require('./handlers/login'));
router.use('/logout', auth.required, require('./handlers/logout'));
router.use('/profile', auth.required, require('./handlers/profile'));
router.use('/planners', auth.required, require('./handlers/planners'));
router.use('/planners/:planner_id/tasks', auth.required, require('./handlers/tasks'));


module.exports = router;