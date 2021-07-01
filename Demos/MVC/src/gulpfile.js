var gulp = require('gulp')
gulp.task('copy', function () {
    return gulp
        .src('./node_modules/@groupdocs.examples.jquery/**')
        .pipe(gulp.dest('./Resources/'))   
})